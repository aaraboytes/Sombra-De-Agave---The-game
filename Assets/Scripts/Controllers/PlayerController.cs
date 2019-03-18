using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    bool paused;
    bool positioned = false;
    public Transform[] positions;
    public MovePoint currentMovePoint;
    int currentPosition = 0;
    public float speed;
    CharacterController body;
    Vector3 movement;
    Vector3 direction;
    Animator anim;

    //Objects
    GameObject itemObj = null;
    Item currentItem = null;

    [Header("Touch")]
    public LayerMask touchDetectionMask;
    public float holdTime;
    float timer;

    [Header("UI")]
    public Slider tequilaFiller;

    private void Start()
    {
        transform.position = new Vector3(positions[0].position.x, transform.position.y, positions[0].position.z);
        body = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        tequilaFiller.gameObject.SetActive(false);
    }
    void Update() {
        if (!paused)
        {
            #region Input controlls PC
            //Input movement
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentPosition--;
                if (currentPosition < 0)
                    currentPosition = positions.Length - 1;
            }else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentPosition++;
                if(currentPosition == positions.Length)
                {
                    currentPosition = 0;
                }
            }
            //Input throw bottles
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if(positioned)
                    tequilaFiller.gameObject.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Z))
            {
                timer = 0;
                tequilaFiller.gameObject.SetActive(false);
            }
            if (Input.GetKey(KeyCode.Z))
            {
                if (positioned)
                {
                    timer += Time.deltaTime;
                    if (timer > holdTime)
                    {
                        TablesManager._instance.ThrowBottle(currentPosition, true);
                        timer = 0;
                    }
                    tequilaFiller.value = timer / holdTime;
                }
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                ActivateItem();
            }
            #endregion
            #region Input controlls Touch
            if (Input.GetMouseButtonDown(0))
            {
                if (positioned)
                {
                    tequilaFiller.gameObject.SetActive(true);
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100.0f, touchDetectionMask))
                    {
                        MovePoint point = hit.collider.gameObject.GetComponent<MovePoint>();
                        if (point != currentMovePoint)
                        {
                            currentMovePoint.Deactivate();
                            currentMovePoint = point;
                            currentPosition = point.Activate();
                        }
                    }
                }
            }else if (Input.GetMouseButtonUp(0))
            {
                timer = 0;
                tequilaFiller.gameObject.SetActive(false);
            }
            if (Input.GetMouseButton(0))
            {
                if (positioned)
                {
                    timer += Time.deltaTime;
                    if (timer > holdTime)
                    {
                        TablesManager._instance.ThrowBottle(currentPosition, true);
                        timer = 0;
                    }
                    tequilaFiller.value = timer / holdTime;
                }
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
        anim.SetFloat("speed", -body.velocity.z);
        #endregion
    }
    private void FixedUpdate()
    {
        Move();
    }
    #region Objects and items
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tequila") && other.GetComponent<Tequila>().returning)
        {
            PickEmptyTequila(other.gameObject);
        }else if (other.CompareTag("Item")) {
            StoreItem(other.gameObject);
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
            UIManager._instance.ItemUsed();
            itemObj.SetActive(true);
            itemObj = null;
            currentItem.Activate();
            currentItem = null;
        }
    }
    void StoreItem(GameObject item)
    {
        if (currentItem == null)
        {
            //Spawn the new item
            itemObj = Pool._instance.SpawnPooledObj(TablesManager._instance.GetItemName(),transform.position,Quaternion.identity);
            itemObj.SetActive(false);
            //Extract item script
            currentItem = itemObj.GetComponent<Item>();
            //Notice to button
            UIManager._instance.ItemAdquired(currentItem);
        }
        //Reinitialize item bottle
        item.GetComponent<Rigidbody>().useGravity = false;
        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
        item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        item.SetActive(false);
    }
    #endregion
    #region Movement
    void Move()
    {
        if(Mathf.Abs(transform.position.z - positions[currentPosition].position.z) > 0.5f)
        {
            Vector3 direction = positions[currentPosition].position;
            direction.y = transform.position.y;
            direction = direction - transform.position;
            movement = direction.normalized * speed * Time.deltaTime;
            if (!body.isGrounded)
                movement.y = Physics.gravity.y;
            positioned = false;
        }
        else
        {
            movement = Vector3.zero;
            positioned = true;
        }
        //Move
        body.Move(movement);
    }
    IEnumerator MoveToPoint()
    {
        while (Mathf.Abs(transform.position.z - positions[currentPosition].position.z) > 0.5f)
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
