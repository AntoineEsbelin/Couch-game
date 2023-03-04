using UnityEngine;

public class BillBoarding : MonoBehaviour
{
    [SerializeField] bool freezeAxisXZ = false;

    private void Update()
    {
        if (freezeAxisXZ)
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        else
            transform.rotation = Camera.main.transform.rotation;
    }
}
