using System;
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
    
    private List<Collider> selectableObjects = new List<Collider>();

    private void Start()
    {
        foreach (Collider collider in FindObjectsOfType<Collider>())
        {
            // 检查物体是否在选择框的范围内
            if (IsInLayerMask(collider.gameObject,selectableLayer))
            {
                Debug.Log(selectableLayer);
                selectableObjects.Add(collider);
            }
        }
    }

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
    
    void SelectObjectsInSelectionBox()
    {
        float width = Mathf.Abs(startPosition.x - endPosition.x);
        float height = Mathf.Abs(startPosition.y - endPosition.y);
        // 获取选择框的二维矩形范围
        Rect selectionRect = new Rect(
            Mathf.Min(startPosition.x, endPosition.x),
            Mathf.Min(startPosition.y, endPosition.y),
            width,
            height
        );

        // selectedObjects.Clear();
  

        // 查找场景中的所有物体
        if (width > 150 && height > 150)
        {
            foreach (Collider collider in selectableObjects)
            {
                // 将物体的世界坐标转换为屏幕坐标
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(collider.transform.position);

                // 检查物体是否在选择框的范围内
                if (selectionRect.Contains(screenPoint))
                {
                    collider.transform.parent.parent.GetComponent<Tower>().selected = true;
                    Debug.Log("Selected: " + collider.name);
                }
                else
                {
                    collider.transform.parent.parent.GetComponent<Tower>().selected = false;
                }
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay((startPosition+endPosition)/2);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
            {
                foreach (Collider collider in selectableObjects)
                {

                    // 选中物体
                    Collider selectedCollider = hit.collider;
                    if (selectedCollider == collider)
                    {
                        collider.transform.parent.parent.GetComponent<Tower>().selected = true;

                        // Debug.Log("Point Selected: " + selectedCollider.name);
                    }
                    else
                    {
                        collider.transform.parent.parent.GetComponent<Tower>().selected = false;

                    }
                
                }
            }
            else
            {
                foreach (Collider collider in selectableObjects)
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
    
    // 判断 GameObject 的 layer 是否包含在 LayerMask 中
    bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) != 0;
    }

}
