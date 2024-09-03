using DG.Tweening;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private bool _isDragging = false;
    private float timeMove = 0.2f;
    private ItemController _controller;
    float _zPos = 0f;

    #region CAMERA ORTHOR
    private void Update()
    {
        //if (_isDragging)
        //{
        //    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    mousePosition.z = -0.2f;
        //    transform.position = mousePosition;
        //}

        if (_isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                transform.position = new Vector3(hit.point.x, hit.point.y, _zPos - 0.25f);
            }
        }
    }

    private void OnMouseDown()
    {
        if (MyGame.Instance.CurrentState != GAMEPLAY_STATE.DRAG_AND_DROP) return;
        if (MyGame.Instance.CurrentItem != null) return;
        if (_controller == null)
        {
            _controller = GetComponent<ItemController>();
        }

        if (_controller.IsEmptyObject()) return;

        MyGame.Instance.CurrentItem = this;
        _zPos = transform.position.z;

        _isDragging = true;
    }

    private void OnMouseUp()
    {
        if (MyGame.Instance.CurrentItem == null) return;
        MyGame.Instance.CurrentItem = null;
        MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.ANIMATION);

        _isDragging = false;

        RaycastHit[] hits = Physics.RaycastAll(transform.position - new Vector3(0, 0, 5f), transform.TransformDirection(Vector3.forward) * 10);

        foreach (RaycastHit hit in hits)
        {
            ItemSlot item = hit.transform.GetComponent<ItemSlot>();
            if (CheckCanPlace(item))
            {
                //Debug.Log("Hit object: " + hit.collider.gameObject.name);
                GetComponentInParent<ItemSlot>().SwapItem(item, () =>
                {
                    MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.CHECK_MATCH);
                });

                return;
            }

        }

        //Debug.Log("not hit other collider");
        //transform.parent = _startParent;
        transform.DOLocalMove(Vector3.zero, timeMove).OnComplete(() =>
        {
            MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.DRAG_AND_DROP);
        });
    }
    #endregion

    //#region CAMERA PERSPECTIVE
    //private Vector3 _offset;

    //private void Update()
    //{
    //    if (_isDragging)
    //    {
    //        // Calculate the ray from the camera through the mouse position
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if (Physics.Raycast(ray, out RaycastHit hit))
    //        {
    //            // Update the position of the object to the hit point
    //            transform.position = hit.point + _offset;
    //        }
    //    }
    //}

    //private void OnMouseDown()
    //{
    //    if (MyGame.Instance.CurrentState != GAMEPLAY_STATE.DRAG_AND_DROP) return;
    //    if (MyGame.Instance.CurrentItem != null) return;
    //    if (_controller == null)
    //    {
    //        _controller = GetComponent<ItemController>();
    //    }

    //    if (_controller.IsEmptyObject()) return;

    //    MyGame.Instance.CurrentItem = this;

    //    // Calculate the offset from the hit point to the object's position
    //    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    _offset = transform.position - mouseWorldPos;

    //    _isDragging = true;
    //}

    //private void OnMouseUp()
    //{
    //    if (MyGame.Instance.CurrentItem == null) return;
    //    MyGame.Instance.CurrentItem = null;
    //    MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.ANIMATION);

    //    _isDragging = false;

    //    // Perform raycast from the object's position forward to detect potential drop zones
    //    Ray ray = new Ray(transform.position, transform.forward);
    //    if (Physics.Raycast(ray, out RaycastHit hit, 10f))
    //    {
    //        ItemSlot item = hit.transform.GetComponent<ItemSlot>();
    //        if (CheckCanPlace(item))
    //        {
    //            GetComponentInParent<ItemSlot>().SwapItem(item, () =>
    //            {
    //                MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.CHECK_MATCH);
    //            });

    //            return;
    //        }
    //    }

    //    // If no valid drop zone is found, return the object to its original position
    //    transform.DOMove(Vector3.zero, timeMove).OnComplete(() =>
    //    {
    //        MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.DRAG_AND_DROP);
    //    });
    //}
    //#endregion

    private bool CheckCanPlace(ItemSlot item)
    {
        if (item == null) return false;
        //Debug.Log("CheckCanPlace: "+ item.name+"/"+ GetComponentInParent<ItemSlot>().name);
        if(item== GetComponentInParent<ItemSlot>()) return false;
        //Debug.Log("CheckCanPlace: CanPlace " + item.CanPlace());
        if (!item.CanPlace()) return false;
        //check layer
        LayerController layer = item.GetComponentInParent<LayerController>();
        if (layer != null && layer.GetNumberSlot()==1)
        {
            return false;
        }

        return true;
    }
}
