using UnityEngine;

using Sirenix.OdinInspector;

public class AnimationTester : MonoBehaviour
{
	private Animator anim;

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.I))
		{
			anim.SetBool("isMoving", false);
			anim.SetBool("isParanoid", false);
		}
		else if(Input.GetKeyDown(KeyCode.P))
		{
			anim.SetBool("isMoving", false);
			anim.SetBool("isParanoid", true);
		}
	}
}