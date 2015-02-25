using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Renderer : MonoBehaviour
{

		public Tree tree{ get; set; }
		public List<List<GNode>> paths { get; set; }

		Material lineMaterial{ get; set; }
	


		private void renderPath ()
		{
				if (paths == null)
						return;

			GL.Color (new Color (0.0f, 0.4f, 0.8f, 1.0f));
			for (int i = 0; i < paths.Count; i++)
					for (int j = 0; j < paths[i].Count - 1; j++) {
							GL.Vertex3(paths[i][j].getPos ().x, 1.0f, paths[i][j].getPos ().z);
							GL.Vertex3 (paths[i][j + 1].getPos ().x, 1.0f, paths[i][j + 1].getPos ().z);
			//GL.Vertex (paths[i][j].getPos ());
							//GL.Vertex (paths[i][j + 1].getPos ());
					}
		
		
		}

		public void CreateLineMaterial ()
		{
				// TODO vad fan gör detta? Hittade på ett forum bara, verkar åtminstone kunna ge färg
				if (!lineMaterial) {
						lineMaterial = new Material ("Shader \"Lines/Colored Blended\" {" +
								"SubShader { Pass { " +
								" BindChannels {" +
								" Bind \"vertex\", vertex Bind \"color\", color }" +
								"} } }");
						lineMaterial.hideFlags = HideFlags.HideAndDontSave;
						lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
				}
		}
	
		private void renderTree ()
		{


				if (tree == null)
						return;

				GL.Color (new Color (1.0f, 0.4f, 0.4f, 1.0f));
				foreach (TNode parent in tree.nodeList) {

						foreach (TNode child in parent.children) {
								GL.Vertex (parent.getPos ());
								GL.Vertex (child.getPos ());
						}
				}
		}

		void OnPostRender ()
		{

				GL.Begin (GL.LINES);
				CreateLineMaterial ();
				lineMaterial.SetPass (0);
				
				renderTree ();
				renderPath ();

				GL.End ();
		}
}
