Shader "Playground/Vertex Color" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
		}
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {}
	}
}