using UnityEngine;

public class CharacterLimitAttribute : PropertyAttribute
{
	public int max = 32;
	public bool enforce = false;

	public CharacterLimitAttribute(int max)
	{
		this.max = max;
		this.enforce = false;
	}

	public CharacterLimitAttribute(int max, bool enforce)
	{
		this.max = max;
		this.enforce = enforce;
	}
}