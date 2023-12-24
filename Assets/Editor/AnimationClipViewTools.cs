// AnimationClipViewTools.cs

// 作者：Drenayo

// 创建日期：2023年12月20日 17:13

// 脚本描述：EditorScene下播放动画，方便调试



// 更新日志：

// 2023年12月20日 17:13 创建脚本

// 2023年12月21日 分层遍历控制器，获取某层所有Clip

// 2023年12月22日 继承OdinEditorWindow重写，支持多人物、多动画同时播放，异步播放，倍速播放，自主选择指定层级，指定层下动画，手动控制播放，统一控制播放等



// 待完成功能：

// 重置按钮与刷新按钮功能重复，重置动画状态应该回到最初始状态（T或A型）（找固定的骨骼？）



using Sirenix.OdinInspector;

using Sirenix.OdinInspector.Editor;

using System.Collections.Generic;

using System.Linq;

using UnityEditor;

using UnityEditor.Animations;

using UnityEngine;



namespace DevCompendium.Tools.Editor

{

    [System.Serializable]

    public class AnimationClipPlay

    {

        public AnimationClipPlay(Animator animator)

        {

            this.animator = animator;

            animC = animator.runtimeAnimatorController as AnimatorController;

            // 获取名字

            objName = animator.gameObject.name;

            ctrlName = animC.name;

            // 获取动画控制器的所有层级

            layerList = animC.layers;

            layerName = layerList[0].name;

            OnLayerValueChange();

            OnStateValueChange();

        }

        private Animator animator;

        private AnimatorController animC;

        private AnimationClip clip;



        [ReadOnly, HideLabel, HorizontalGroup("name")]

        public string objName = "游戏对象";

        [ReadOnly, HideLabel, HorizontalGroup("name")]

        public string ctrlName = "控制器";



        [ListDrawerSettings(ShowIndexLabels = true), HideInInspector] // 动画状态列表

        public List<AnimatorState> animatorStateList;



        [ListDrawerSettings(ShowIndexLabels = true), HideInInspector] // 层级列表

        public AnimatorControllerLayer[] layerList;



        [ValueDropdown("GetAllLayer"), OnValueChanged("OnLayerValueChange"), LabelText("层级"), HorizontalGroup("dd")]

        public string layerName;



        [ValueDropdown("GetAllState"), OnValueChanged("OnStateValueChange"), LabelText("动画片段"), HorizontalGroup("dd")]

        public string stateName;



        [LabelText("进度"), ProgressBar(0, 1), OnValueChanged("OnProgressChange")]

        public float progress = 0;



        [LabelText("倍速"), ProgressBar(0.2, 3)]

        public float doubleSpeed = 1;



        [LabelText("循环")]

        public bool isLooping = false;



        [HideInInspector]

        public bool isPlaying = false;



        // 时间戳，用于计算播放进度，计算记录进度偏移

        [HideInInspector]

        public float elapsedTime = 0f;



        [Button("播放"), ButtonGroup]

        public void Btn_Play()

        {

            elapsedTime = (float)EditorApplication.timeSinceStartup;

            isPlaying = true;

        }



        [Button("停止"), ButtonGroup]

        public void Btn_Stop()

        {

            isPlaying = false;

        }



        [Button("重置"), ButtonGroup]

        public void Btn_Reste()

        {

            Btn_Stop();

            layerName = layerList[0].name;

            OnLayerValueChange();

            OnStateValueChange();

            clip.SampleAnimation(animator.gameObject, clip.length * 0);

        }







        // 循环

        public void Loop()

        {

            if (isPlaying)

            {

                float deltaTime = (float)(EditorApplication.timeSinceStartup - elapsedTime);



                // 根据倍速累加进度

                progress += deltaTime * doubleSpeed;



                // 如果选择循环，并且进度大于等于1，则重置为0

                if (progress >= 1f)

                {

                    progress = 0f;

                    if (!isLooping)

                    {

                        isPlaying = false;

                    }

                }



                // 更新上次调用的时间

                elapsedTime = (float)EditorApplication.timeSinceStartup;

                OnProgressChange();

            }

        }



        private List<string> GetAllState()

        {

            return animatorStateList.Select(an => an.name).ToList();

        }



        private List<string> GetAllLayer()

        {

            return layerList.Select(lay => lay.name).ToList();

        }



        // 改变图层

        private void OnLayerValueChange()

        {

            // 遍历获取所有动画

            animatorStateList = WalkStateMachine(layerList.FirstOrDefault(lay => lay.name == layerName).stateMachine, layerName, new List<AnimatorState>());

            // 默认显示第一个

            stateName = animatorStateList[0].name;

        }



        // 改变动画

        private void OnStateValueChange()

        {

            clip = animatorStateList.FirstOrDefault(state => state.name == stateName).motion as AnimationClip;

        }



        // 进度改变

        private void OnProgressChange()

        {

            if (clip != null)

                clip.SampleAnimation(animator.gameObject, clip.length * progress);

        }



        /// <summary>

        /// 递归遍历AnimatorStateMachine的所有Clip, list参数是为了递归存值，传个空即可new List

        /// </summary>

        public List<AnimatorState> WalkStateMachine(AnimatorStateMachine stateMachine, string layerName, List<AnimatorState> list)

        {

            // 遍历当前状态机的所有子状态机

            foreach (ChildAnimatorStateMachine childStateMachine in stateMachine.stateMachines)

            {

                WalkStateMachine(childStateMachine.stateMachine, $"{layerName}/{childStateMachine.stateMachine.name}", list);

            }



            // 遍历当前状态机的所有动画Clip

            foreach (ChildAnimatorState state in stateMachine.states)

            {

                AnimationClip clip = state.state.motion as AnimationClip;

                // 动画控制器上的节点上的clip不为空，就加入进来

                if (clip != null)

                {

                    // Debug.Log($"Clip found at {layerName}/{state.state.name}: {clip.name}");

                    list.Add(state.state);

                }

            }

            return list;

        }



    }



    public class AnimationClipViewTools : OdinEditorWindow

    {

        [MenuItem("Tools/图谱工具集/动画预览工具(施工完毕)")]

        public static void Open()

        {

            var win = GetWindow<AnimationClipViewTools>();

            win.titleContent = new GUIContent("动画预览工具");

            //win.minSize = new Vector2(450, 400);

            //win.maxSize = new Vector2(450, 400);

            Debug.Log("[动画预览工具]使用方法：选择Hierarchy的对象(可多选)，然后点击刷新。");

        }



        [LabelText("播放列表")]

        public List<AnimationClipPlay> animationClipPlays;



        [Space, HideLabel, ReadOnly]

        public string info = "统一控制";



        [Button("播放"), ButtonGroup]

        public void Btn_Play()

        {

            if (animationClipPlays != null && animationClipPlays.Count > 0)

            {

                foreach (var item in animationClipPlays)

                {

                    item.Btn_Play();

                }

            }

        }



        [Button("停止"), ButtonGroup]

        public void Btn_Stop()

        {

            if (animationClipPlays != null && animationClipPlays.Count > 0)

            {

                foreach (var item in animationClipPlays)

                {

                    item.Btn_Stop();

                }

            }

        }



        [Button("重置"), ButtonGroup]

        public void Btn_Reste()

        {

            if (animationClipPlays != null && animationClipPlays.Count > 0)

            {

                foreach (var item in animationClipPlays)

                {

                    item.Btn_Reste();

                }

            }

        }



        [Button("刷新"), ButtonGroup]

        public void Btn_Refresh()

        {

            ResetLoad();

        }





        public void ResetLoad()

        {

            animationClipPlays.Clear();

            // 遍历选中的物体

            if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)

            {

                GameObject[] gos = Selection.gameObjects;

                List<Animator> antorList = new List<Animator>();

                foreach (var go in gos)

                {

                    if (go.GetComponentInChildren<Animator>(true) != null)

                        animationClipPlays.Add(new AnimationClipPlay(go.GetComponentInChildren<Animator>(true)));

                }

            }

        }

        [System.Obsolete]
        protected override void OnGUI()
        {
            base.OnGUI();

            if (animationClipPlays != null && animationClipPlays.Count > 0)
            {

                foreach (var item in animationClipPlays)
                {

                    item.Loop();
                }
            }
            Repaint();

        }

    }

}