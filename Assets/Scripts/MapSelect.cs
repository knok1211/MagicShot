using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MapSelect : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // UI 위에서 클릭한 경우 무시
            if (IsPointerOverUI())
            {
                return;
            }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Vector3 hitPoint = hit.point;
                    int x = Mathf.RoundToInt(hitPoint.x);
                    int z = Mathf.RoundToInt(hitPoint.z);

                    Debug.Log($"선택한 좌표: X={x}, Z={z}");

                    if (GameController.Instance != null)
                    {
                        GameController.Instance.SetSelectedPosition(x, z);
                    }
                    break;
                }
            }
        }
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        return results.Count > 0;
    }
}
