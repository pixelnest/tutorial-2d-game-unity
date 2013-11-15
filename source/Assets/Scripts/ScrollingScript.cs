using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Parallax scrolling script that should be assigned to a layer
/// </summary>
public class ScrollingScript : MonoBehaviour
{
  /// <summary>
  /// Scrolling speed
  /// </summary>
  public Vector2 speed = new Vector2(10, 10);

  /// <summary>
  /// Moving direction
  /// </summary>
  public Vector2 direction = new Vector2(-1, 0);

  /// <summary>
  /// Movement should be applied to camera
  /// </summary>
  public bool isLinkedToCamera = false;

  /// <summary>
  /// Background is inifnite
  /// </summary>
  public bool isLooping = false;

  private List<Transform> backgroundPart;

  void Start()
  {
    // For infinite background only
    if (isLooping)
    {
      // Get all part of the layer
      backgroundPart = new List<Transform>();

      for (int i = 0; i < transform.childCount; i++)
      {
        Transform child = transform.GetChild(i);

        // Only visible children
        if (child.renderer != null)
        {
          backgroundPart.Add(child);
        }
      }

      // Sort by position 
      // REM: left from right here, we would need to add few conditions to handle all scrolling directions
      backgroundPart = backgroundPart.OrderBy(t => t.position.x).ToList();
    }
  }

  void Update()
  {
    // Movement
    Vector3 movement = new Vector3(
      speed.x * direction.x,
      speed.y * direction.y,
      0);

    movement *= Time.deltaTime;
    transform.Translate(movement);

    // Move the camera
    if (isLinkedToCamera)
    {
      Camera.main.transform.Translate(movement);
    }

    // Loop
    if (isLooping)
    {
      // Get the first object
      Transform firstChild = backgroundPart.FirstOrDefault();

      if (firstChild != null)
      {
        // Check if we are after the camera
        // Position first as IsVisibleFrom is a heavy method
        if (firstChild.position.x < Camera.main.transform.position.x)
        {
          if (firstChild.renderer.IsVisibleFrom(Camera.main) == false)
          {
            // Get the last positions
            Transform lastChild = backgroundPart.LastOrDefault();
            Vector3 lastPosition = lastChild.transform.position;
            Vector3 lastSize = (lastChild.renderer.bounds.max - lastChild.renderer.bounds.min);

            // Set position after
            // REM: here too it works for horizontal scrolling only
            firstChild.position = new Vector3(lastPosition.x + lastSize.x, firstChild.position.y, firstChild.position.z);

            // Set as last
            backgroundPart.Remove(firstChild);
            backgroundPart.Add(firstChild);
          }
        }
      }
    }
  }
}