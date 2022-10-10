using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {
	const float MinPathUpdateTime = 0.2f;
	const float PathUpdateMoveThreshold = 	0.5f;
	public Transform target;
	public float speed = 20;
	public float TurnSpeed = 3;
	public float TurnDist = 5;
	Path path;

	void Start()
	{
		StartCoroutine(UpdatePath());
	}
	public void OnPathFound(Vector3[] Waypoints, bool pathSuccessful) {
		if (pathSuccessful) {
			path = new Path(Waypoints, transform.position, TurnDist);
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator UpdatePath(){
		if(Time.timeSinceLevelLoad<0.3f)
		{
			yield return new WaitForSeconds(0.3f);
		}
		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
		float SqrMoveThshld = PathUpdateMoveThreshold * PathUpdateMoveThreshold;
		Vector3 TargetPosOld = target.position;

		while(true)
		{
			yield return new WaitForSeconds(MinPathUpdateTime);
			if((target.position- TargetPosOld).sqrMagnitude > SqrMoveThshld){
				PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
				TargetPosOld = target.position;
			}
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