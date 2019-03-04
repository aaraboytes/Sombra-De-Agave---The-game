using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public SwipeManager swipeControlls;
    public LayerMask touchDetectionMask;
    public Transform[] positions;
    public MovePoint currentMovePoint;
    public float speed;
    CharacterController body;
    Vector3 movement;
    Vector3 direction;
    Animator anim;
    int currentPosition = 0;
    bool paused;
    [SerializeField]
    Item currentItem = null;


    private void Start()
    {
        transform.position = new Vector3(positions[0].position.x, transform.position.y, positions[0].position.z);
        body = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }
    void Update() {
        if (!paused)
        {
            #region Input controlls
            //Input movement
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                StopAllCoroutines();
                currentPosition--;
                if (currentPosition < 0)
                    currentPosition = positions.Length - 1;
                StartCoroutine("MoveToPoint");
            }else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StopAllCoroutines();
                currentPosition++;
                if(currentPosition == positions.Length)
                {
                    currentPosition = 0;
                }
                StartCoroutine("MoveToPoint");
            }else if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 100.0f, touchDetectionMask))
                {
                    MovePoint point = hit.collider.gameObject.GetComponent<MovePoint>();
                    if (point != currentMovePoint)
                    {
                        currentMovePoint.Deactivate();
                        currentMovePoint = point;
                        currentPosition = point.Activate();
                        StopAllCoroutines();
                        StartCoroutine("MoveToPoint");
                    }
                }
            }
            //Input throw bottles
            if (Input.GetKeyDown(KeyCode.Z) || swipeControlls.SwipeRight)
            {
                TablesManager._instance.ThrowBottle(currentPosition,true);
            }
            if (Input.GetKeyDown(KeyCode.X) || swipeControlls.SwipeLeft)
            {
                ActivateItem();
            }
            #endregion
            #region Player direction
            //Direction
            if (movement.z > 0)
                direction = Vector3.forward;
            else if (movement.z < 0)
                direction = -Vector3.forward;
            else if (movement.x > 0)
                direction = Vector3.right;
            else if (movement.x < 0)
                direction = -Vector3.right;
            #endregion
        }
        #region Animation parameters
        //Animation
        if(paused)
            anim.SetBool("moving", false);
        else
            anim.SetBool("moving", Mathf.Abs(movement.x) + Mathf.Abs(movement.z) != 0);
        anim.SetInteger("right", (int)-Input.GetAxisRaw("Horizontal"));
        anim.SetInteger("up", (int)Input.GetAxisRaw("Vertical"));
        #endregion
    }
    #region Objects and items
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tequila") && other.GetComponent<Tequila>().returning)
        {
            PickEmptyTequila(other.gameObject);
        }else if (other.CompareTag("Item")) {
            StoreItem(other.gameObject);
            UIManager._instance.ItemAdquired(currentItem);
        }
    }
    void PickEmptyTequila(GameObject tequila)
    {
        tequila.GetComponent<Tequila>().Reinitialize();
        tequila.SetActive(false);
    }
    public void ActivateItem()
    {
        if (currentItem)
        {
            currentItem.Activate();
            currentItem = null;
            UIManager._instance.ItemUsed();
        }
    }
    void StoreItem(GameObject item)
    {
        if (currentItem == null || currentItem.name != item.GetComponent<Item>().name)
        {
            if(currentItem!= null)
                currentItem.gameObject.SetActive(false);
            currentItem = item.GetComponent<Item>();
            item.transform.rotation = Quaternion.identity;
            item.GetComponent<Rigidbody>().velocity = Vector3.zero;
            item.GetComponent<Rigidbody>().useGravity = false;
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().isTrigger = true;
            if(item.GetComponent<MeshRenderer>())
                item.GetComponent<MeshRenderer>().enabled = false;
            if (item.transform.childCount > 0)
            {
                for (int i = 0; i < item.transform.childCount; i++)
                {
                    item.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            item.SetActive(false);
        }
    }
    #endregion
    #region Gameplay methods
    IEnumerator MoveToPoint()
    {
        while (Vector3.Distance(transform.position, positions[currentPosition].position) > 0.5f)
        {
            Vector3 direction = positions[currentPosition].position;
            direction.y = transform.position.y;
            direction = direction - transform.position;
            movement = direction.normalized * speed * Time.deltaTime;
            if (!body.isGrounded)
                movement.y = Physics.gravity.y;
            //Move
            body.Move(movement);
            yield return null;
        }
    }
    #endregion
    #region Getters & setters
    public int Position { get { return currentPosition; } }
    #endregion
}
