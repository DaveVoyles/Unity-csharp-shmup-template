using UnityEngine;

public class Player : MonoBehaviour
{


    public Transform     myTransform;	               // for caching
    public GameManager   gameManager;                  // used to call all global game related functions
    public float        playerSpeed;
    public float        horizontalMovementLimit;	   // stops the player leaving the view
    public float        verticalMovementLimit;
    public  float        fireRate          = 0.05f;    // time between shots
    public  float        playerBulletSpeed = 25;
    private float        nextFire          = 0;        // used to time the next shot


    void Start()
    {
        myTransform = transform;                       // caching the transform is faster than accessing 'transform' directly
    }

    void Update()
    {
        // read movement inputs
        var horizontalMove = (playerSpeed * Input.GetAxis("Horizontal"))    * Time.deltaTime;
        var verticalMove   = (playerSpeed * Input.GetAxis("Vertical"))      * Time.deltaTime;
        var moveVector = new Vector3(horizontalMove, 0, verticalMove);
            moveVector = Vector3.ClampMagnitude(moveVector, playerSpeed     * Time.deltaTime); // prevents the player moving above its max speed on diagonals

        // move the player
        myTransform.Translate(moveVector);


        KeepPlayerInBounds();
        CheckIfShooting();
    }

    private void KeepPlayerInBounds()
    {
        // restrict the position to inside the player's movement limits
        myTransform.position = new Vector3(Mathf.Clamp(myTransform.position.x, -horizontalMovementLimit, horizontalMovementLimit), 0,
                                           Mathf.Clamp(myTransform.position.z, -verticalMovementLimit, verticalMovementLimit));
    }

    private void CheckIfShooting()
    {
        // shooting
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            // delay the next shot by the firing rate
            nextFire = Time.time + fireRate;
            
            // get a bullet from the stack
            Bullet newBullet = GameManager.playerBulletStack.Pop();

            // position and enable it
            newBullet.gameObject.transform.position = myTransform.position;
            newBullet.gameObject.active = true;

            // set its speed (it moves in its own onUpdate function)
            newBullet.motion = new Vector3(0, 0, playerBulletSpeed);
        }

    }

    void OnTriggerEnter(Collider other) // must have hit an enemy or enemy bullet
    {
        gameManager.lives--;                                         // lose a life            

        // check if it was a bullet we hit, if so put it back on its stack
        if (other.CompareTag("EnemyBullet"))
        {
            GameManager.enemyBulletStack.Push(other.GetComponent<Bullet>());
            other.gameObject.SetActive(false);                       // deactivate the bullet
        }
        else if (other.CompareTag("Enemy"))                          // if it was an enemy, just destroy it
        {
            other.GetComponent<Enemy>().Explode();
        }
    }
}