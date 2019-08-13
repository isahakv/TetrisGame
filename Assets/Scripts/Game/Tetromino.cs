using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    private AudioSource audioSource;

    public GameObject rotationOffset;
    public GameObject[] minos;
    public bool allowRotation = true, limitRotation = false;
    public AudioClip moveSound, rotateSound, landSound;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Move(Vector2 move)
    {
        transform.position += new Vector3(move.x, move.y, 0.0f);
        audioSource.PlayOneShot(moveSound);
    }

    public void Rotate()
    {
        if (!allowRotation)
            return;

        if (limitRotation)
        {
            if (rotationOffset.transform.localRotation.eulerAngles.z >= 90.0f) // If rotation is 90 then rotate by -90.
                rotationOffset.transform.Rotate(new Vector3(0.0f, 0.0f, -90.0f));
            else                                                               // If rotation is 0 then rotate by 90.
                rotationOffset.transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
        }
        else
            rotationOffset.transform.Rotate(new Vector3(0.0f, 0.0f, -90.0f));

        audioSource.PlayOneShot(rotateSound);
    }

    public void OnLanded()
    {
        audioSource.PlayOneShot(landSound);
    }

    public bool IsParentOf(GameObject mino)
    {
        foreach (GameObject m in minos)
        {
            if (m == mino)
                return true;
        }

        return false;
    }
}
