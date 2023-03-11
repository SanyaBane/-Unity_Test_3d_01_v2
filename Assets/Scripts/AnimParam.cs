using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class AnimParam
    {
        public AnimatorControllerParameterType type;
        public string paramName;
        private object data;

        public AnimParam(Animator anim, string paramName, AnimatorControllerParameterType type)
        {
            this.type = type;
            this.paramName = paramName;
            switch (type)
            {
                case AnimatorControllerParameterType.Int:
                    this.data = (int)anim.GetInteger(paramName);
                    break;

                case AnimatorControllerParameterType.Float:
                    this.data = (float)anim.GetFloat(paramName);
                    break;

                case AnimatorControllerParameterType.Bool:
                    this.data = (bool)anim.GetBool(paramName);
                    break;
            }
        }

        public object getData()
        {
            return data;
        }

        public static List<AnimParam> GetListOfAnimParams(Animator animator)
        {
            List<AnimParam> animParams = new List<AnimParam>();
            for (int i = 0; i < animator.parameters.Length; i++)
            {
                AnimatorControllerParameter p = animator.parameters[i];
                AnimParam ap = new AnimParam(animator, p.name, p.type);
                animParams.Add(ap);
            }

            return animParams;
        }

        public static void SetAnimParams(Animator animator, List<AnimParam> animParams)
        {
            foreach (AnimParam p in animParams)
            {
                switch (p.type)
                {
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(p.paramName, (int)p.getData());
                        break;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(p.paramName, (float)p.getData());
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(p.paramName, (bool)p.getData());
                        break;
                }
            }

            animParams.Clear();
        }
    }
}
