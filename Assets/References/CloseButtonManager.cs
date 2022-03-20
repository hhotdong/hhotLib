//using System.Collections.Generic;
//using UnityEngine;

//public class CloseButtonManager : MonoBehaviour
//{
//	private static CloseButtonManager instance;
//	public static CloseButtonManager Instance
//	{
//		get
//		{
//			if (instance == null)
//				instance = FindObjectOfType<CloseButtonManager>();
//			return instance;
//		}
//	}

//	private readonly Stack<ToggleCloseButton> m_CloseButtonsStack = new Stack<ToggleCloseButton>();


//	//////////////////////////////////////////
//	// Initialize
//	//////////////////////////////////////////
    
//	private void Start()
//    {
//		if (m_CloseButtonsStack.Count > 0)
//			m_CloseButtonsStack.Clear();
//	}


//	//////////////////////////////////////////
//	// Utilities
//	//////////////////////////////////////////

//	public void Push(ToggleCloseButton closeBtn)
//    {
//		if (closeBtn == null)
//			return;

//		if (m_CloseButtonsStack.Count > 0)
//		{
//			var peek = m_CloseButtonsStack.Peek();
//			if (peek == closeBtn)
//				return;
//            else
//			    peek.Toggle(false);
//		}

//		m_CloseButtonsStack.Push(closeBtn);
//	}

//    public void Pop()
//    {
//		if (m_CloseButtonsStack.Count < 1)
//			return;

//		m_CloseButtonsStack.Pop();

//		if (m_CloseButtonsStack.Count > 0)
//			m_CloseButtonsStack.Peek().Toggle(true);
//	}
//}
