using UnityEngine;

/// <summary>
/// Enemy generic behavior
/// </summary>
public class BossScript : MonoBehaviour
{
  private bool hasSpawn;

  //  Component references
  private MoveScript moveScript;
  private WeaponScript[] weapons;
  private Animator animator;
  private SpriteRenderer[] renderers;

  // Boss pattern (bot really an AI)
  public float minAttackCooldown = 0.5f;
  public float maxAttackCooldown = 2f;

  private float aiCooldown;
  private bool isAttacking;
  private Vector2 positionTarget;

  void Awake()
  {
    // Retrieve the weapon only once
    weapons = GetComponentsInChildren<WeaponScript>();

    // Retrieve scripts to disable when not spawn
    moveScript = GetComponent<MoveScript>();

    // Get the animator
    animator = GetComponent<Animator>();

    // Get the renderers in children
    renderers = GetComponentsInChildren<SpriteRenderer>();
  }

  void Start()
  {
    hasSpawn = false;

    // Disable everything
    // -- collider
    collider2D.enabled = false;
    // -- Moving
    moveScript.enabled = false;
    // -- Shooting
    foreach (WeaponScript weapon in weapons)
    {
      weapon.enabled = false;
    }

    // Default behavior
    isAttacking = false;
    aiCooldown = maxAttackCooldown;
  }

  void Update()
  {
    // Check if the enemy has spawned
    if (hasSpawn == false)
    {
      // We check only the first renderer for simplicity.
      // But we don't know if it's the body, and eye or the mouth...
      if (renderers[0].IsVisibleFrom(Camera.main))
      {
        Spawn();
      }
    }
    else
    {
      // AI
      //------------------------------------
      // Move or attack. permute. Repeat.
      aiCooldown -= Time.deltaTime;

      if (aiCooldown <= 0f)
      {
        isAttacking = !isAttacking;
        aiCooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        positionTarget = Vector2.zero;

        // Set or unset the attack animation
        animator.SetBool("Attack", isAttacking);
      }

      // Attack
      //----------
      if (isAttacking)
      {
        // Stop any movement
        moveScript.direction = Vector2.zero;

        foreach (WeaponScript weapon in weapons)
        {
          if (weapon != null && weapon.enabled && weapon.CanAttack)
          {
            weapon.Attack(true);
            SoundEffectsHelper.Instance.MakeEnemyShotSound();
          }
        }
      }
      // Move
      //----------
      else
      {
        // Define a target?
        if (positionTarget == Vector2.zero)
        {
          // Get a point on the screen, convert to world
          Vector2 randomPoint = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

          positionTarget = Camera.main.ViewportToWorldPoint(randomPoint);
        }

        // Are we at the target? If so, find a new one
        if (collider2D.OverlapPoint(positionTarget))
        {
          // Reset, will be set at the next frame
          positionTarget = Vector2.zero;
        }

        // Go to the point
        Vector3 direction = ((Vector3)positionTarget - this.transform.position);

        // Remember to use the move script
        moveScript.direction = Vector3.Normalize(direction);
      }
    }
  }

  private void Spawn()
  {
    hasSpawn = true;

    // Enable everything
    // -- Collider
    collider2D.enabled = true;
    // -- Moving
    moveScript.enabled = true;
    // -- Shooting
    foreach (WeaponScript weapon in weapons)
    {
      weapon.enabled = true;
    }

    // Stop the main scrolling
    foreach (ScrollingScript scrolling in FindObjectsOfType<ScrollingScript>())
    {
      if (scrolling.isLinkedToCamera)
      {
        scrolling.speed = Vector2.zero;
      }
    }
  }

  void OnTriggerEnter2D(Collider2D otherCollider2D)
  {
    // Taking damage? Change animation
    ShotScript shot = otherCollider2D.gameObject.GetComponent<ShotScript>();
    if (shot != null)
    {
      if (shot.isEnemyShot == false)
      {
        // Stop attacks and start moving awya
        aiCooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        isAttacking = false;

        // Change animation
        animator.SetTrigger("Hit");
      }
    }
  }

  void OnDrawGizmos()
  {
    // A little tip: you can display debug information in your scene with Gizmos
    if (hasSpawn && isAttacking == false)
    {
      Gizmos.DrawSphere(positionTarget, 0.5f);
    }
  }
}