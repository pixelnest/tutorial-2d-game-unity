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
  private Vector2 repeatableSize;
  private Vector3 gap = Vector3.zero;

  void Start()
  {
    // For infinite background only
    if (isLooping)
    {
      //---------------------------------------------------------------------------------
      // 1 - Retrieve background objects
      // -- We need to know what this background is made of
      // -- Store a reference of each object
      // -- Order those items in the order of the scrolling, so we know the item that will be the first to be recycled
      // -- Compute the relative position between each part before they start moving
      //---------------------------------------------------------------------------------

      // Get all part of the layer
      backgroundPart = new List<Transform>();

      for (int i = 0; i < transform.childCount; i++)
      {
        Transform child = transform.GetChild(i);

        // Only visible children
        if (child.renderer != null)
        {
          backgroundPart.Add(child);
          
          // First element
          if (backgroundPart.Count == 1)
          {
            // Gap is the space between zero and the first element. 
            // We need it when we loop.
            gap = child.transform.position;
            
            if (direction.x == 0) gap.x = 0;
            if (direction.y == 0) gap.y = 0;
          }
        }
      }

      if (backgroundPart.Count == 0)
      {
        Debug.LogError("Nothing to scroll!");
      }

      // Sort by position 
      // -- Depends on the scrolling direction
      backgroundPart = backgroundPart.OrderBy(t => t.position.x * (-1 * direction.x)).ThenBy(t => t.position.y * (-1 * direction.y)).ToList();

      // Get the size of the repeatable parts
      var first = backgroundPart.First();
      var last = backgroundPart.Last();

      repeatableSize = new Vector2(
        Mathf.Abs(last.position.x - first.position.x),
        Mathf.Abs(last.position.y - first.position.y)
        );
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
      //---------------------------------------------------------------------------------
      // 2 - Check if the object is before, in or after the camera bounds
      //---------------------------------------------------------------------------------

      // Camera borders
      var dist = (transform.position - Camera.main.transform.position).z;
      float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
      float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
//      float width = Mathf.Abs(rightBorder - leftBorder);

      var topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
      var bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
//      float height = Mathf.Abs(topBorder - bottomBorder);

      // Determine entry and exit border using direction
      Vector3 exitBorder = Vector3.zero;
      Vector3 entryBorder = Vector3.zero;

      if (direction.x < 0)
      {
        exitBorder.x = leftBorder;
        entryBorder.x = rightBorder;
      }
      else if (direction.x > 0)
      {
        exitBorder.x = rightBorder;
        entryBorder.x = leftBorder;
      }

      if (direction.y < 0)
      {
        exitBorder.y = bottomBorder;
        entryBorder.y = topBorder;
      }
      else if (direction.y > 0)
      {
        exitBorder.y = topBorder;
        entryBorder.y = bottomBorder;
      }

      // Get the first object
      Transform firstChild = backgroundPart.FirstOrDefault();

      if (firstChild != null)
      {
        bool checkVisible = false;

        // Check if we are after the camera
        // The check is on the position first as IsVisibleFrom is a heavy method
        // Here again, we check the border depending on the direction
        if (direction.x != 0)
        {
          if ((direction.x < 0 && (firstChild.position.x < exitBorder.x))
          || (direction.x > 0 && (firstChild.position.x > exitBorder.x)))
          {
            checkVisible = true;
          }
        }
        if (direction.y != 0)
        {
          if ((direction.y < 0 && (firstChild.position.y < exitBorder.y))
          || (direction.y > 0 && (firstChild.position.y > exitBorder.y)))
          {
            checkVisible = true;
          }
        }

        // Check if the sprite is really visible on the camera or not
        if (checkVisible)
        {
          //---------------------------------------------------------------------------------
          // 3 - The object was in the camera bounds but isn't anymore.
          // -- We need to recycle it
          // -- That means he was the first, he's now the last
          // -- And we physically moves him to the further position possible
          //---------------------------------------------------------------------------------

          if (firstChild.renderer.IsVisibleFrom(Camera.main) == false)
          {
            // Set position in the end
            firstChild.position = gap + new Vector3(
              firstChild.position.x + ((repeatableSize.x + firstChild.renderer.bounds.size.x) * -1 * direction.x),
              firstChild.position.y + ((repeatableSize.y + firstChild.renderer.bounds.size.y) * -1 * direction.y),
              firstChild.position.z
              );

            // The first part become the last one
            backgroundPart.Remove(firstChild);
            backgroundPart.Add(firstChild);
          }
        }
      }

    }
  }
}
