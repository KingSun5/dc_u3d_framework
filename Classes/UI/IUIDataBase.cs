using UnityEngine;
using System.Collections;

public interface IUIDataBase
{
	void Init ();
	void Release ();

	void RegisterEvent();
	void UnRegisterEvent();
}
