using UnityEngine;

/// <summary>
/// Handle hitpoints and damages
/// </summary>
public class HealthScript : MonoBehaviour
{
  /// <summary>
  /// Total hitpoints
  /// </summary>
  public int HP = 2;

  /// <summary>
  /// Enemy or player?
  /// </summary>
  public bool IsEnemy = true;

  void OnTriggerEnter2D(Collider2D collider)
  {
    // Is this a shot?
    ShotScript shot = collider.gameObject.GetComponent<ShotScript>();
    if (shot != null)
    {
      // Avoid friendly fire
      if (shot.IsEnemyShot != IsEnemy)
      {
        HP -= shot.Damage;

        // Destroy the shot
        Destroy(shot.gameObject); // Remember to always target the game object, otherwise you will just remove the script

        if (HP <= 0)
        {
          // Explosion!
          SpecialEffectsHelper.Instance.Explosion(transform.position);
          SoundEffectsHelper.Instance.MakeExplosionSound();

          // Dead!
          Destroy(gameObject);
        }
      }
    }
  }
}