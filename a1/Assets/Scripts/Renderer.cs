using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Renderer : MonoBehaviour
{

		public Tree tree{ get; set; }
		public List<GNode> path { get; set; }

		Material lineMaterial{ get; set; }
	

		private void renderPath ()
		{
				if (path == null)
						return;

				GL.Color (new Color (0.2f, 0.2f, 0.8f, 1.0f));
				for (int i = 0; i < path.Count - 1; i++) {
						GL.Vertex (path [i].getPos ());
						GL.Vertex (path [i + 1].getPos ());
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

				GL.Color (new Color (0.7f, 0.2f, 0.5f, 1.0f));
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
