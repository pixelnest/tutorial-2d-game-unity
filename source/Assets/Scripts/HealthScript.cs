using UnityEngine;

/// <summary>
/// Handle hitpoints and damages
/// </summary>
public class HealthScript : MonoBehaviour
{
  /// <summary>
  /// Total hitpoints
  /// </summary>
  public int hp = 1;

  /// <summary>
  /// Enemy or player?
  /// </summary>
  public bool isEnemy = true;

  /// <summary>
  /// Inflicts damage and check if the object should be destroyed
  /// </summary>
  /// <param name="damageCount"></param>
  public void Damage(int damageCount)
  {
    hp -= damageCount;

    if (hp <= 0)
    {
      // Explosion!
      SpecialEffectsHelper.Instance.Explosion(transform.position);
      SoundEffectsHelper.Instance.MakeExplosionSound();

      // Dead!
      Destroy(gameObject);
    }
  }

  void OnTriggerEnter2D(Collider2D otherCollider)
  {
    // Is this a shot?
    ShotScript shot = otherCollider.gameObject.GetComponent<ShotScript>();
    if (shot != null)
    {
      // Avoid friendly fire
      if (shot.isEnemyShot != isEnemy)
      {
        Damage(shot.damage);

        // Destroy the shot
        Destroy(shot.gameObject); // Remember to always target the game object, otherwise you will just remove the script
      }
    }
  }
}