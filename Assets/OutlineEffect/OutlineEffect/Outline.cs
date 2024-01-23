using UnityEngine;

namespace AssetBuilder
{
	[RequireComponent(typeof(Renderer))]
	/* [ExecuteInEditMode] */
	public class Outline : MonoBehaviour
	{
		public Renderer Renderer { get; private set; }
		public SpriteRenderer SpriteRenderer { get; private set; }
		public SkinnedMeshRenderer SkinnedMeshRenderer { get; private set; }
		public MeshFilter MeshFilter { get; private set; }

		public int color;
		public bool eraseRenderer;


		public OutlineAnimation outlineAnimation;

		private void Awake()
		{
			Renderer = GetComponent<Renderer>();
			SkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
			SpriteRenderer = GetComponent<SpriteRenderer>();
			MeshFilter = GetComponent<MeshFilter>();
		}

		void OnEnable()
		{
			OutlineEffect.Instance?.AddOutline(this);
			outlineAnimation.enabled = true;
		}

		void OnDisable()
		{
			OutlineEffect.Instance?.RemoveOutline(this);
		}

		private Material[] _SharedMaterials;
		public Material[] SharedMaterials
		{
			get
			{
				if (_SharedMaterials == null)
					_SharedMaterials = Renderer.sharedMaterials;

				return _SharedMaterials;
			}
		}
	}
}
