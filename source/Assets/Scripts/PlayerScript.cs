using UnityEngine;

/// <summary>
/// Player controller and behavior
/// </summary>
public class PlayerScript : MonoBehaviour
{
  /// <summary>
  /// 0 - The speed of the ship
  /// </summary>
  public Vector2 speed = new Vector2(25, 25);

  // 1 - Store the movement
  private Vector2 movement;

  void Update()
  {
    // 2 - Retrieve axis information
    float inputX = Input.GetAxis("Horizontal");
    float inputY = Input.GetAxis("Vertical");

    // 3 - Movement per direction
    movement = new Vector2(
      speed.x * inputX,
      speed.y * inputY);

    // 5 - Shooting
    bool shoot = Input.GetButtonDown("Fire1");
    shoot |= Input.GetButtonDown("Fire2"); // For Mac users, ctrl + arrow is a bad idea

    if (shoot)
    {
      WeaponScript weapon = GetComponent<WeaponScript>();
      if (weapon != null && weapon.CanAttack)
      {
        weapon.Attack(false);
        SoundEffectsHelper.Instance.MakePlayerShotSound();
      }
    }

    // 6 - Make sure we are not outside the camera bounds
    var dist = (transform.position - Camera.main.transform.position).z;
    var leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
    var rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
    var topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
    var bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;

    transform.position = new Vector3(
              Mathf.Clamp(transform.position.x, leftBorder, rightBorder),
              Mathf.Clamp(transform.position.y, topBorder, bottomBorder),
              transform.position.z
              ); 
  }

  void FixedUpdate() 
  {
	// 4 - Move the game object
	rigidbody2D.velocity = movement;
  }

  void OnDestroy()
  {
    // Game Over
    // Add it to the parent, as this game object is likely to be destroyed immediately
    transform.parent.gameObject.AddComponent<GameOverScript>();
  }
}