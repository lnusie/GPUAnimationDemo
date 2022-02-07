using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.EditorCoroutines.Editor;

public class GPUVertexAnimatonGenerator
{
    private static float SampleFactor = 1 / 60f;
    [MenuItem("Assets/GenGPUVertexAnimaton")]
    public static void GPUVertexAnimaton()
    {
        var selector = UnityEditor.Selection.activeObject;
        if(selector == null)
        {
            return;
        }

        EditorCoroutineUtility.StartCoroutineOwnerless(GenThings(selector));
    }

    static IEnumerator GenThings(Object selector)
    {
        var path = AssetDatabase.GetAssetPath(selector);
        var dir = "Assets/Prefabs";
        var originPrefab = (GameObject)GameObject.Instantiate(selector);
        originPrefab.name = selector.name;
        var animator = originPrefab.GetComponent<Animator>();
        if(animator == null)
        {
            GameObject.DestroyImmediate(originPrefab);
            Debug.LogError("selection prefab does not have Animator component!");
            yield break;
        }

        
        var skin = originPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
        var modelMesh = new Mesh();
        skin.BakeMesh(modelMesh);
        var uvs = new Vector2[modelMesh.vertices.Length];
        for (int k = 0; k < modelMesh.vertices.Length; k++)
        {
            uvs[k] = new Vector2(k, 0.0f);
        }
        modelMesh.uv3 = uvs;

        var folderName = "GPUAnimation";
        var parentFolder = Path.Combine(dir, folderName);   //   Assets/Prefab/GPU
        var subFolder = Path.Combine(parentFolder, originPrefab.name);  //   Assets/Prefab/GPU/name
        var animationFolder = Path.Combine(subFolder, "Animations");  //   Assets/Prefab/GPU/name/Animations

        if (Directory.Exists(subFolder))
        {
            FileUtil.DeleteFileOrDirectory(subFolder);
            AssetDatabase.Refresh();
        }

        if (Directory.Exists(animationFolder))
        {
            FileUtil.DeleteFileOrDirectory(animationFolder);
            AssetDatabase.Refresh(); 
        }

        AssetDatabase.CreateFolder(parentFolder, originPrefab.name);
        var savePath = Path.Combine(subFolder, string.Format("{0}_mesh.asset", originPrefab.name));

        AssetDatabase.CreateAsset(modelMesh, savePath);  //保存mesh

        var vCount = skin.sharedMesh.vertexCount;
        var texWidth = vCount;
        var totalFrame = 0;
        UnityEditor.Animations.AnimatorController animatorController = (UnityEditor.Animations.AnimatorController)animator.runtimeAnimatorController;
        UnityEditor.Animations.AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
        for (int i = 0; i < stateMachine.states.Length; i++)
        {
            AnimationClip clip = stateMachine.states[i].state.motion as AnimationClip;
            totalFrame += (int)(clip.length / SampleFactor) + 1;
        }

        var mesh = new Mesh();
        var defaultState = stateMachine.defaultState;
        Texture2D posTex = new Texture2D(texWidth, totalFrame, TextureFormat.RGBAHalf, false);
        posTex.wrapMode = TextureWrapMode.Clamp;
        posTex.filterMode = FilterMode.Point;
        animator.speed = 0;  //因为靠代码手动Update，所以speed设置为0.
        float boundMin = float.MaxValue;
        float boundMax = float.MinValue;
        int frameOffset = 0;
        List<AnimationInfo> animInfos = new List<AnimationInfo>();
        List<List<Color>> pixels = new List<List<Color>>();
        for (int i = 0; i < stateMachine.states.Length; i++)
        {
            UnityEditor.Animations.AnimatorState state = stateMachine.states[i].state;
            AnimationClip clip = state.motion as AnimationClip;
            var thisClipFrames = (int)(clip.length / SampleFactor) + 1;
            animator.Play(state.name);
            for (int j = 0; j < thisClipFrames; j++)
            {
                List<Color> pos = new List<Color>();
                animator.Play(state.name, 0, (float)j / thisClipFrames);
                animator.Update(Time.deltaTime);
                yield return null;
                skin.BakeMesh(mesh);//将skinMesh瞬间的位置烘焙进Mesh中

                for (int z = 0; z < mesh.vertexCount; z++)
                {
                    Vector3 vertex = mesh.vertices[z];
                    Color col = new Color(vertex.x, vertex.y, vertex.z);
                    pos.Add(col);
                    boundMin = Mathf.Min(boundMin, vertex.x, vertex.y, vertex.z);
                    boundMax = Mathf.Max(boundMax, vertex.x, vertex.y, vertex.z);
                }
                pixels.Add(pos);
                if (j == 1 || j == thisClipFrames - 1)
                {
                    //在第一帧和最后帧分别插入一行，防止动画切换时采样错误
                    pixels.Add(pos);
                }
            }


            AnimationInfo info = AnimationInfo.CreateInstance<AnimationInfo>();
            info.m_AnimFrameOffset = frameOffset+1;
            info.m_TotalFrames = totalFrame;
            info.m_AnimFrameNum = thisClipFrames;
            info.m_boundMax = boundMax;
            info.m_boundMin = boundMin;
            info.m_AnimLength = clip.length;
            info.m_AnimName = state.name;
            info.m_Loop = clip.isLooping;
            info.m_IsDefault = (state == defaultState);
            animInfos.Add(info);
            var animationSavePath = Path.Combine(animationFolder, string.Format("{0}.asset", info.m_AnimName));
            AssetDatabase.CreateAsset(info, animationSavePath);

            frameOffset += thisClipFrames;
        }

        var diff = boundMax - boundMin;
        for (int i = 0; i < pixels.Count; i++)
        {
            var item = pixels[i];
            for (int j = 0; j < item.Count; j++)
            {
                var pixel = item[j];
                pixel.r = (pixel.r - boundMin) / diff;
                pixel.g = (pixel.g - boundMin) / diff;
                pixel.b = (pixel.b - boundMin) / diff;
                posTex.SetPixel(j, i, pixel); //u为第几个顶点，v为第几帧
            }
        }

        posTex.Apply();
        AssetDatabase.CreateAsset(posTex, Path.Combine(subFolder,  string.Format("{0}_pos.asset", originPrefab.name)));  //保存位置纹理
        File.WriteAllBytes(Path.Combine(subFolder, string.Format("{0}_pos.png", originPrefab.name)), posTex.EncodeToPNG());

        Shader shader = Shader.Find("Custom/GPUAnimation");
        Material mat = new Material(shader);
        mat.CopyPropertiesFromMaterial(skin.sharedMaterial);
        var mat_path = Path.Combine(subFolder,  string.Format("{0}_mat.mat", originPrefab.name));
        AssetDatabase.CreateAsset(mat, mat_path);  //保存材质

        Material tempMat = AssetDatabase.LoadAssetAtPath(mat_path, typeof(Material)) as Material;
        for (int i = 0; i < animInfos.Count; i++)
        {
            if(animInfos[i].m_IsDefault)
            {
                tempMat.SetFloat("_BoundMax", animInfos[i].m_boundMax);
                tempMat.SetFloat("_BoundMin", animInfos[i].m_boundMin);
                tempMat.SetTexture("_PosTex", posTex);
                break;
            }
        }
        tempMat.enableInstancing = true;
        AssetDatabase.SaveAssets();



        GameObject newPrefab = new GameObject(originPrefab.name);
        GameObject model = new GameObject("model");
        model.transform.parent = newPrefab.transform;
        model.transform.localScale = Vector3.one;
        model.transform.localPosition = Vector3.zero;
        model.transform.localEulerAngles = Vector3.zero;
        model.AddComponent<MeshFilter>().sharedMesh = modelMesh;
        model.AddComponent<MeshRenderer>().sharedMaterial = mat;
        GPUAnimatiorController controller = model.AddComponent<GPUAnimatiorController>();
        controller.m_AnimInfos = animInfos.ToArray();

        PrefabUtility.SaveAsPrefabAsset(newPrefab, Path.Combine(subFolder, string.Format("{0}_prefab.prefab", originPrefab.name)));

        GameObject.DestroyImmediate(originPrefab);
        GameObject.DestroyImmediate(newPrefab);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
