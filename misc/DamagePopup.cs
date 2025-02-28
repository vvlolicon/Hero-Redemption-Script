using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMesh textMesh;
    private Color textColor;
    private Camera cam;

    private float dissapearTimer = 0.5f;
    private float fadeOutSpeed = 5f;

    private Vector3 offset;

    private float moveSpeed = 150f;

	public void SetUp(string dmgText, Color textC, Vector2 offset)
    {
        textMesh = GetComponent<TextMesh>();
        cam = Camera.main;
        textColor = textC;
        textMesh.text = dmgText;
        textMesh.color = textColor;

        // speed = the ratio of x and y(y is positive)
        float moveXSpeed = offset.x/offset.y;
        float moveYSpeed = offset.y/offset.x;
        this.offset = new Vector3(moveXSpeed, Mathf.Abs(moveYSpeed), 0);
        this.offset.Normalize();
    }

	private void LateUpdate()
	{
        if (textMesh != null)
        {

            transform.LookAt(2 * transform.position - cam.transform.position);
            Vector3 newPos = RandomMethods.ScreenPointOffset(cam, transform.position, offset * moveSpeed * Time.deltaTime);
            transform.position = newPos;

            dissapearTimer -= Time.deltaTime;
            if (dissapearTimer <= 0f)
            {
                textColor.a -= fadeOutSpeed * Time.deltaTime;
                textMesh.color = textColor;
                if (textColor.a <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }
	}
}
