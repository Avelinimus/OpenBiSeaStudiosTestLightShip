namespace MVC.Unity.Editor
{
    public class BehaviourEditor
    {
        public object target { get; set; }

        public virtual void OnInspectorGUI()
        {

        }

        public virtual void OnHeaderGUI()
        {

        }

        public void Repaint()
        {
            
        }
    }

    public class BehaviourEditor<T> : BehaviourEditor where T : class
    {
        public T Target { get { return target as T; } }
    }
}
