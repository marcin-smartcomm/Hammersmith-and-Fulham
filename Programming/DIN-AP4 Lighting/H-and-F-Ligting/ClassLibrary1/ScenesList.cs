using System.Collections.Generic;

namespace H_and_F_Lighting
{
    public class Scene
    {
        public string sceneName { get; set; }
        public uint sceneNum { get; set; }
    }
    public class ScenesList
    {
        public List<Scene> availableScenes { get; set; }
    }
}
