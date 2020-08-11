using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Precision")]
    public bool m_isPrecision;
    private Vector3 m_offset;
    private RectTransform m_rt;

    public int xShift;
    public int yShift;

    //Vector2 = (x, z)
    public Vector2 center;
    public Vector2 buttonLeft;

    private float half_width;
    private float half_height;
    private int rect_half_width;
    private int rect_half_height;

    void Awake()
    {
        m_rt = gameObject.GetComponent<RectTransform>();
        half_height = Mathf.Abs(center.x - buttonLeft.x);
        half_width = Mathf.Abs(center.y - buttonLeft.y);
        rect_half_width = Mathf.FloorToInt(m_rt.rect.width / 2);
        rect_half_height = Mathf.FloorToInt(m_rt.rect.height / 2);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_isPrecision)
        {
            Vector3 tWorldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out tWorldPos);
            m_offset = transform.position - tWorldPos;
        }
        else
        {
            m_offset = Vector3.zero;
        }

        SetDraggedPosition(eventData);
    }

    private void OnEnable()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        float down = -(center.x - player.position.x) / half_width;
        float left = (center.y - player.position.z) / half_height;
        m_rt.localPosition = new Vector3(rect_half_width * left, rect_half_height * down, 0);
        CheckForBoundary();
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            m_rt.position = globalMousePos + m_offset;
        }
    }

    private void CheckForBoundary()
    {
        if (m_rt.anchoredPosition.x > xShift)
        {
            m_rt.anchoredPosition = new Vector2(xShift, m_rt.anchoredPosition.y);
        }
        else if (m_rt.anchoredPosition.x < -xShift)
        {
            m_rt.anchoredPosition = new Vector2(-xShift, m_rt.anchoredPosition.y);
        }
        if (m_rt.anchoredPosition.y > yShift)
        {
            m_rt.anchoredPosition = new Vector2(m_rt.anchoredPosition.x, yShift);
        }
        else if (m_rt.anchoredPosition.y < -yShift)
        {
            m_rt.anchoredPosition = new Vector2(m_rt.anchoredPosition.x, -yShift);
        }
    }
    void Update()
    {
         CheckForBoundary();
    }
}