using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {


	public Transform target;
	public float speed = 20;
	public float TurnSpeed = 3;
	public float TurnDist = 5;
	Path path;


	public void OnPathFound(Vector3[] Waypoints, bool pathSuccessful) {
		if (pathSuccessful) {
			path = new Path(Waypoints, transform.position, TurnDist);
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath() {
		bool FollowingPath = true;
		int PathIndex = 0;
		transform.LookAt(path.LookPoints[0]);

		while (FollowingPath) {
			Vector2 Pos2D = new Vector2(transform.position.x, transform.position.z);
			while(path.TurnBoundaries[PathIndex].HasPointCrossedLine(Pos2D))
			{
				//Reached path end
				if(PathIndex == path.FinishLineIndex)
				{
					FollowingPath = false;
					break;
				}
				else{
					PathIndex++;
				}
			}

			if(FollowingPath)
			{
				Quaternion TargetRotation = Quaternion.LookRotation(path.LookPoints[PathIndex] - transform.position);
				transform.rotation = Quaternion.Lerp(transform.rotation, TargetRotation, Time.deltaTime * TurnSpeed);
				transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
			}
			yield return null;

		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			path.DrawWithGizmos();
		}
	}
}