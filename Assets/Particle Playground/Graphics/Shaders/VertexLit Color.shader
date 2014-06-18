Shader "Playground/Vertex Lit Color" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting On
		ColorMaterial AmbientAndDiffuse
		Zwrite On
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "normal", normal
		}
		Material {
			Diffuse [_Color]
			Ambient [_Color]
			Emission[_Vertex]
		}
		Pass {
			
		}
	}
}