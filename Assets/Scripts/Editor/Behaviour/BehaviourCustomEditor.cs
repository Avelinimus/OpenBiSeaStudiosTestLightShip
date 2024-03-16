using System;

namespace MVC.Unity.Editor
{
    public class BehaviourCustomEditor : Attribute
    {
        private readonly Type m_InspectedType;
        private readonly bool m_EditorForChildClasses;

        public BehaviourCustomEditor(Type inspectedType)
        {
            m_InspectedType = inspectedType;
        }

        public BehaviourCustomEditor(Type inspectedType, bool editorForChildClasses)
        {
            m_InspectedType = inspectedType;
            m_EditorForChildClasses = editorForChildClasses;
        }

        public Type InspectedType
        {
            get { return m_InspectedType; }
        }

        public bool EditorForChildClasses
        {
            get { return m_EditorForChildClasses; }
        }
    }
}
