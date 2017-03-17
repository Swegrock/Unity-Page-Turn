using UnityEngine;
using System.Collections;

public class Book : MonoBehaviour {

	private GameObject clicker;
	private GameObject page;
	private GameObject turningPage;
	private bool turning;

	void Start () {
		Vector3 scale = transform.localScale;
		transform.localScale = Vector3.one;

		clicker = new GameObject ();
		clicker.name = "clicker";
		clicker.transform.parent = transform;
		clicker.AddComponent<CircleCollider2D> ();
		clicker.transform.localPosition = new Vector3 (GetComponent<Renderer> ().bounds.size.x/2, -GetComponent<Renderer> ().bounds.size.y/2, 0);
		clicker.GetComponent<CircleCollider2D> ().radius = 0.2f;
		clicker.layer = LayerMask.NameToLayer ("UI");

		page = GameObject.CreatePrimitive (PrimitiveType.Quad);
		page.transform.localScale = new Vector3 (GetComponent<Renderer> ().bounds.size.x / 2, GetComponent<Renderer> ().bounds.size.y);
		page.transform.parent = transform;
		page.transform.localPosition = new Vector3 (GetComponent<Renderer> ().bounds.size.x / 4, 0, 0);

		EdgeCollider2D ec = gameObject.AddComponent<EdgeCollider2D> ();
		Vector3 bottomRight = new Vector3 (GetComponent<Renderer> ().bounds.size.x/2,-GetComponent<Renderer> ().bounds.size.y/2,transform.position.z);
		Vector3 topRight = new Vector3 (GetComponent<Renderer> ().bounds.size.x/2,GetComponent<Renderer> ().bounds.size.y/2 + 1,transform.position.z);
		Vector3 top = new Vector3 (0,GetComponent<Renderer> ().bounds.size.y/2 + 1,transform.position.z);
		Vector3 topLeft = new Vector3 (-GetComponent<Renderer> ().bounds.size.x/2,GetComponent<Renderer> ().bounds.size.y/2 + 1,transform.position.z);
		Vector3 bottomLeft = new Vector3 (-GetComponent<Renderer> ().bounds.size.x/2,-GetComponent<Renderer> ().bounds.size.y/2,transform.position.z);
		ec.points = new Vector2[] {bottomLeft,bottomRight,topRight,top,topLeft};

		transform.localScale = scale;
	}

	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
			if (hit.collider != null && hit.collider.name == "clicker") { 
				turning = true;
			} 
		} else if (Input.GetMouseButtonUp (0)) {
			if (turning) {
				turning = false;
			}
		}
		if (turning) {
			Vector3 mousePoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePoint.z = 0;
			clicker.transform.position = mousePoint;
		} else {
			if (clicker.transform.position.x < transform.position.x - GetComponent<Renderer> ().bounds.size.x / 4) {
				clicker.transform.position = Vector3.Lerp (clicker.transform.position, new Vector3 (-GetComponent<Renderer> ().bounds.size.x/2,-GetComponent<Renderer> ().bounds.size.y/2,transform.position.z) , Time.deltaTime*3);
			} else {
				clicker.transform.position = Vector3.Lerp (clicker.transform.position, new Vector3 (GetComponent<Renderer> ().bounds.size.x/2,-GetComponent<Renderer> ().bounds.size.y/2,transform.position.z) , Time.deltaTime*3);
			}
		}
	}

	void LateUpdate() {
		Vector3 center = new Vector3 (transform.position.x,transform.position.y-GetComponent<Renderer> ().bounds.size.y/2,transform.position.z);
		Vector3 topRight = new Vector3 (transform.position.x+GetComponent<Renderer> ().bounds.size.x/2,transform.position.y+GetComponent<Renderer> ().bounds.size.y/2,transform.position.z);
		float distanceCTR = Vector3.Distance (center, topRight);

		float maxDistance = GetComponent<Renderer> ().bounds.size.x/2;
		float distance = Vector3.Distance(center, clicker.transform.position);
		if (distance > maxDistance)
		{
			Vector3 vect =  center - clicker.transform.position;
			vect = vect.normalized;
			vect *= (distance-maxDistance);
			clicker.transform.position += vect;
		}
		if (clicker.transform.position.y < center.y)
			clicker.transform.position = new Vector3 (clicker.transform.position.x,center.y);

		float dist = Vector3.Distance (clicker.transform.position, new Vector3 (GetComponent<Renderer> ().bounds.size.x / 2, -GetComponent<Renderer> ().bounds.size.y / 2, transform.position.z));

		Vector3 midPoint = Vector3.Lerp (clicker.transform.position, new Vector3 (GetComponent<Renderer> ().bounds.size.x / 2, -GetComponent<Renderer> ().bounds.size.y / 2, transform.position.z),0.5f);

		var newVec = midPoint-new Vector3 (GetComponent<Renderer> ().bounds.size.x / 2, -GetComponent<Renderer> ().bounds.size.y / 2, transform.position.z);
		var newVector = Vector3.Cross (newVec, Vector3.forward);
		newVector.Normalize();

		var newPoint = GetComponent<Renderer> ().bounds.size.y*2*newVector+midPoint;
		var newPoint2 = -GetComponent<Renderer> ().bounds.size.y*2*newVector+midPoint;
		//Debug.DrawLine (newPoint,newPoint2);

		if (clicker.transform.position.x < center.x)
			newPoint.x -= center.x - clicker.transform.position.x;

		float distance2 = Vector3.Distance(center, newPoint);
		if (distance > distanceCTR)
		{
			Vector3 vect =  center - newPoint;
			vect = vect.normalized;
			vect *= (distance2-distanceCTR);
			newPoint += vect;
		}

		RaycastHit2D hit = Physics2D.Linecast (midPoint, newPoint, ~(1 << LayerMask.NameToLayer ("UI")));
		RaycastHit2D hit2 = Physics2D.Linecast (midPoint, newPoint2, ~(1 << LayerMask.NameToLayer ("UI")));
		if (hit.collider != null && hit2.collider != null) {
			if (hit.point.y < topRight.y) {
				Debug.DrawLine (clicker.transform.position, hit.point);
				Debug.DrawLine (clicker.transform.position, hit2.point);
				Debug.DrawLine (hit.point, hit2.point);
			} else {
				if (clicker.transform.position.x < center.x) {
					topRight = new Vector3 (transform.position.x + GetComponent<Renderer> ().bounds.size.x / 2, transform.position.y + GetComponent<Renderer> ().bounds.size.y / 2, transform.position.z);
					topRight.x -= Mathf.Abs(clicker.transform.position.x*2) - GetComponent<Renderer> ().bounds.size.x / 2;
				}
				Debug.DrawLine (clicker.transform.position, hit.point);
				Debug.DrawLine (clicker.transform.position, hit2.point);
				Debug.DrawLine (hit.point, topRight);
				Debug.DrawLine (topRight, hit2.point);
			}
		}
	}
}