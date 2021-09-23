using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChoiceController : MonoBehaviour {

    private bool isDragged = false;

    private Vector3 originalPosition;

    private Vector3 originalScale;

    private Vector3 dragScale;

    private ColorChoiceController() {
        isDragged = false;
        dragScale = new Vector3(0.75f, 0.75f, 0.75f);
    }

    protected void Start () {
        originalPosition = transform.position;
        originalScale = transform.localScale;
    }

    protected void Update () {
        if (isDragged) {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = -1;
            transform.position = p;
        }
    }

    protected void OnMouseDown() {
        // Debug.Log("Color choice: " + name);
        isDragged = true;
        transform.localScale = dragScale;
    }

    protected void OnMouseUp() {
        // Debug.Log("Current color choice released");
        isDragged = false;
        GameManager.GetGameManager().DropOn(transform.position, gameObject);
        transform.position = originalPosition;
        transform.localScale = originalScale;
    }
}
