using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public RectTransform selectionBox;   // UI选择框的RectTransform
    public Canvas canvas;                // UI Canvas
    public LayerMask selectableLayer;    // 可选择的物体层

    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool isSelecting = false;
    private Vector3 center;
    private Vector3 size;
    
    private List<Collider> selectedObjects = new List<Collider>();

    void Update()
    {
        HandleSelection();
        // HandlePointSelection();

    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0)) // 鼠标左键按下
        {
            isSelecting = true;
            startPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0)) // 鼠标左键抬起
        {
            isSelecting = false;
            SelectObjectsInSelectionBox();
            selectionBox.gameObject.SetActive(false); // 隐藏选择框
        }

        if (isSelecting)
        {
            endPosition = Input.mousePosition;
            UpdateSelectionBox();
        }
    }

    void UpdateSelectionBox()
    {
        if (!selectionBox.gameObject.activeSelf)
            selectionBox.gameObject.SetActive(true);

        Vector2 size = endPosition - startPosition;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
        selectionBox.anchoredPosition = ScreenToLocalPoint(startPosition) + size / 2;
    }

    void SelectObjectsInSelectionBox_old()
    {
        // 转换屏幕坐标到世界坐标（近平面）
        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(startPosition.x, startPosition.y, Camera.main.nearClipPlane));
        // 转换屏幕坐标到世界坐标（远平面）
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(endPosition.x, endPosition.y, Camera.main.farClipPlane));

        // 计算选择框的中心和大小
        center = (screenBottomLeft + screenTopRight) / 2;
        size = new Vector3(Mathf.Abs(screenTopRight.x - screenBottomLeft.x), Mathf.Abs(screenTopRight.y - screenBottomLeft.y), Camera.main.farClipPlane - Camera.main.nearClipPlane);
        Debug.Log(screenBottomLeft);
        Debug.Log(screenTopRight);
        Debug.Log(size);
        // 使用 OverlapBox 来检测物体
        Collider[] colliders = Physics.OverlapBox(center, size / 2, Quaternion.identity, selectableLayer);

        
        foreach (Collider collider in colliders)
        {
            // 这里可以添加选中物体的逻辑，比如高亮显示
            Debug.Log("Selected: " + collider.name);

            if (collider.transform.parent.parent.GetComponent<Tower>().selected == false)
            {
                collider.transform.parent.parent.GetComponent<Tower>().selected = true;
                collider.gameObject.GetComponent<Renderer>().material.color = Color.red;;
            }
            else
            {
                collider.transform.parent.parent.GetComponent<Tower>().selected = false;
                collider.gameObject.GetComponent<Renderer>().material.color = Color.blue;;
            }
        }
    }

    void SelectObjectsInSelectionBox()
    {
        // 获取选择框的二维矩形范围
        Rect selectionRect = new Rect(
            Mathf.Min(startPosition.x, endPosition.x),
            Mathf.Min(startPosition.y, endPosition.y),
            Mathf.Abs(startPosition.x - endPosition.x),
            Mathf.Abs(startPosition.y - endPosition.y)
        );

        selectedObjects.Clear();

        // 查找场景中的所有物体
        foreach (Collider collider in FindObjectsOfType<Collider>())
        {
            // 将物体的世界坐标转换为屏幕坐标
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(collider.transform.position);

            // 检查物体是否在选择框的范围内
            if (collider.gameObject.layer==3)
            {
                if (selectionRect.Contains(screenPoint))
                {
                    collider.transform.parent.parent.GetComponent<Tower>().selected = true;
                    selectedObjects.Add(collider);
                    Debug.Log("Selected: " + collider.name);
                }
                else
                {
                    collider.transform.parent.parent.GetComponent<Tower>().selected = false;
                }
            }
        }
    }
    Vector2 ScreenToLocalPoint(Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(selectionBox.parent as RectTransform, screenPoint, canvas.worldCamera, out Vector2 localPoint);
        return localPoint;
    }
    
    void HandlePointSelection()
    {
        if (Input.GetMouseButtonDown(0)) // 鼠标左键按下
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
            {
                // 选中物体
                Collider selectedCollider = hit.collider;
                if (selectedCollider.gameObject.layer == 3)
                {
                    if (selectedCollider != null)
                    {
                        selectedCollider.transform.parent.parent.GetComponent<Tower>().selected = true;

                        Debug.Log("Point Selected: " + selectedCollider.name);
                    }
                }
            }
        }
    }

}
