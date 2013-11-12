using UnityEngine;

/// <summary>
/// Simply moves the current game object
/// </summary>
public class MoveScript : MonoBehaviour
{
  // 0 - Designer variables

  /// <summary>
  /// Projectile speed
  /// </summary>
  public Vector2 Speed = new Vector2(10, 10);

  /// <summary>
  /// Moving direction
  /// </summary>
  public Vector2 Direction = new Vector2(-1, 0);

  void Update()
  {
    // 1 - Movement
    Vector3 movement = new Vector3(
      Speed.x * Direction.x,
      Speed.y * Direction.y,
      0);

    movement *= Time.deltaTime;
    transform.Translate(movement);
  }
}