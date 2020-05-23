// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:0,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:True,rprd:True,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5441177,fgcg:0.4627208,fgcb:0.4440961,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2553,x:37761,y:30991,varname:node_2553,prsc:2|diff-3830-OUT,spec-2311-OUT,gloss-8025-OUT,normal-3745-OUT,emission-1131-OUT,transm-5971-OUT,lwrap-5971-OUT,amdfl-221-OUT,amspl-1131-OUT,alpha-3155-OUT,refract-3275-OUT,voffset-3816-OUT;n:type:ShaderForge.SFN_Color,id:9757,x:33131,y:30455,ptovrint:False,ptlb:Water Color,ptin:_WaterColor,varname:node_9757,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2941176,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:1285,x:33594,y:30383,varname:node_1285,prsc:2|IN-6434-OUT,IMIN-9504-OUT,IMAX-6014-OUT,OMIN-3478-OUT,OMAX-94-OUT;n:type:ShaderForge.SFN_Vector1,id:9504,x:33265,y:30338,varname:node_9504,prsc:2,v1:-0.1;n:type:ShaderForge.SFN_SceneDepth,id:9346,x:33005,y:30014,varname:node_9346,prsc:2;n:type:ShaderForge.SFN_Depth,id:7285,x:32977,y:30221,varname:node_7285,prsc:2;n:type:ShaderForge.SFN_Subtract,id:747,x:33276,y:30043,varname:node_747,prsc:2|A-9346-OUT,B-7285-OUT;n:type:ShaderForge.SFN_Multiply,id:6434,x:33536,y:29982,varname:node_6434,prsc:2|A-747-OUT,B-1541-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1541,x:33446,y:30212,ptovrint:False,ptlb:Density,ptin:_Density,varname:node_1541,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Vector1,id:94,x:33479,y:30559,varname:node_94,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:3478,x:33493,y:30612,varname:node_3478,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:6014,x:33326,y:30491,varname:node_6014,prsc:2|A-9757-RGB,B-5526-OUT;n:type:ShaderForge.SFN_Clamp01,id:3439,x:33830,y:30383,varname:node_3439,prsc:2|IN-1285-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8584,x:33830,y:30667,ptovrint:False,ptlb:Fade Level,ptin:_FadeLevel,varname:node_8584,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Power,id:969,x:34095,y:30519,varname:node_969,prsc:2|VAL-3439-OUT,EXP-8584-OUT;n:type:ShaderForge.SFN_SceneColor,id:5382,x:34057,y:30703,varname:node_5382,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1055,x:34909,y:30498,varname:node_1055,prsc:2|A-2981-OUT,B-5382-RGB;n:type:ShaderForge.SFN_Slider,id:6538,x:34652,y:29999,ptovrint:False,ptlb:Specularity,ptin:_Specularity,varname:node_6538,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2dAsset,id:9873,x:34882,y:28759,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_9873,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:60977704998116b4dbd3350b7b9c4bae,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:1516,x:35152,y:28667,varname:node_1516,prsc:2,tex:60977704998116b4dbd3350b7b9c4bae,ntxv:0,isnm:False|UVIN-5018-UVOUT,TEX-9873-TEX;n:type:ShaderForge.SFN_Tex2d,id:5988,x:35152,y:28831,varname:node_5988,prsc:2,tex:60977704998116b4dbd3350b7b9c4bae,ntxv:0,isnm:False|UVIN-9281-UVOUT,TEX-9873-TEX;n:type:ShaderForge.SFN_ObjectScale,id:6772,x:33693,y:28678,varname:node_6772,prsc:2,rcp:False;n:type:ShaderForge.SFN_Append,id:5384,x:33893,y:28700,varname:node_5384,prsc:2|A-6772-X,B-6772-Z;n:type:ShaderForge.SFN_TexCoord,id:9634,x:33757,y:28886,varname:node_9634,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:7657,x:33992,y:28871,varname:node_7657,prsc:2|A-5384-OUT,B-9634-UVOUT,C-1891-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1891,x:33757,y:29066,ptovrint:False,ptlb:Scale,ptin:_Scale,varname:node_1891,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:3286,x:34143,y:28687,varname:node_3286,prsc:2|A-896-OUT,B-7657-OUT;n:type:ShaderForge.SFN_Vector1,id:896,x:34096,y:28589,varname:node_896,prsc:2,v1:0.666;n:type:ShaderForge.SFN_Add,id:8852,x:34321,y:28587,varname:node_8852,prsc:2|A-5187-OUT,B-3286-OUT;n:type:ShaderForge.SFN_Panner,id:9281,x:34530,y:28827,varname:node_9281,prsc:2,spu:1,spv:-1|UVIN-7657-OUT,DIST-4302-OUT;n:type:ShaderForge.SFN_Panner,id:5018,x:34530,y:28665,varname:node_5018,prsc:2,spu:-1,spv:1|UVIN-8852-OUT,DIST-4302-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9511,x:34004,y:29090,ptovrint:False,ptlb:Water Speed,ptin:_WaterSpeed,varname:node_9511,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Time,id:7509,x:34004,y:29182,varname:node_7509,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4302,x:34261,y:29116,varname:node_4302,prsc:2|A-9511-OUT,B-7509-TSL;n:type:ShaderForge.SFN_Add,id:8905,x:35400,y:28684,varname:node_8905,prsc:2|A-1516-R,B-5988-R;n:type:ShaderForge.SFN_Add,id:3450,x:35400,y:28820,varname:node_3450,prsc:2|A-1516-G,B-5988-G;n:type:ShaderForge.SFN_Append,id:8876,x:35622,y:28728,varname:node_8876,prsc:2|A-8905-OUT,B-3450-OUT,C-1800-OUT;n:type:ShaderForge.SFN_Vector1,id:1800,x:35584,y:28886,varname:node_1800,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:3745,x:35033,y:29375,varname:node_3745,prsc:2|A-34-OUT,B-8876-OUT,T-5389-OUT;n:type:ShaderForge.SFN_Slider,id:5389,x:35746,y:28878,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_5389,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:2;n:type:ShaderForge.SFN_Vector3,id:34,x:35820,y:28568,varname:node_34,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Multiply,id:2311,x:35214,y:30108,varname:node_2311,prsc:2|A-6538-OUT,B-1628-OUT;n:type:ShaderForge.SFN_Fresnel,id:1628,x:34556,y:30064,varname:node_1628,prsc:2|EXP-2533-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2533,x:34544,y:30233,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_2533,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_DepthBlend,id:1033,x:35482,y:30780,varname:node_1033,prsc:2|DIST-149-OUT;n:type:ShaderForge.SFN_ValueProperty,id:149,x:34577,y:30838,ptovrint:False,ptlb:Shore Opacity,ptin:_ShoreOpacity,varname:node_149,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tau,id:5526,x:33274,y:30718,varname:node_5526,prsc:2;n:type:ShaderForge.SFN_Add,id:1210,x:34306,y:30508,varname:node_1210,prsc:2|A-969-OUT,B-6967-OUT;n:type:ShaderForge.SFN_DepthBlend,id:9070,x:33873,y:29997,varname:node_9070,prsc:2|DIST-5807-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5807,x:33799,y:29929,ptovrint:False,ptlb:Shore Transparency,ptin:_ShoreTransparency,varname:node_5807,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:6;n:type:ShaderForge.SFN_Clamp01,id:2981,x:34501,y:30490,varname:node_2981,prsc:2|IN-1210-OUT;n:type:ShaderForge.SFN_OneMinus,id:6967,x:34001,y:30206,varname:node_6967,prsc:2|IN-9070-OUT;n:type:ShaderForge.SFN_Vector1,id:5971,x:35430,y:30443,varname:node_5971,prsc:2,v1:1;n:type:ShaderForge.SFN_ComponentMask,id:7283,x:34909,y:30675,varname:node_7283,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-3745-OUT;n:type:ShaderForge.SFN_Multiply,id:3275,x:35121,y:30773,varname:node_3275,prsc:2|A-7283-OUT,B-1204-OUT,C-15-OUT;n:type:ShaderForge.SFN_Slider,id:1999,x:34831,y:30941,ptovrint:False,ptlb:Refraction Intensity,ptin:_RefractionIntensity,varname:node_1999,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_RemapRange,id:1204,x:35170,y:30939,varname:node_1204,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.1|IN-1999-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:3209,x:36644,y:29369,ptovrint:False,ptlb:Foam,ptin:_Foam,varname:node_3209,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4569f84b7d2064c48ad37bc2748ca835,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2758,x:36936,y:29316,varname:node_2758,prsc:2,tex:4569f84b7d2064c48ad37bc2748ca835,ntxv:0,isnm:False|UVIN-8859-UVOUT,TEX-3209-TEX;n:type:ShaderForge.SFN_Tex2d,id:5223,x:36936,y:29453,varname:node_5223,prsc:2,tex:4569f84b7d2064c48ad37bc2748ca835,ntxv:0,isnm:False|UVIN-5346-UVOUT,TEX-3209-TEX;n:type:ShaderForge.SFN_ObjectScale,id:6913,x:35866,y:29774,varname:node_6913,prsc:2,rcp:False;n:type:ShaderForge.SFN_ValueProperty,id:8163,x:35854,y:29956,ptovrint:False,ptlb:Foam Scale,ptin:_FoamScale,varname:node_8163,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Append,id:320,x:36032,y:29805,varname:node_320,prsc:2|A-6913-X,B-6913-Z;n:type:ShaderForge.SFN_TexCoord,id:6273,x:36004,y:29956,varname:node_6273,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:2369,x:36183,y:29868,varname:node_2369,prsc:2|A-320-OUT,B-6273-UVOUT,C-8163-OUT;n:type:ShaderForge.SFN_Multiply,id:6142,x:36418,y:29884,varname:node_6142,prsc:2|A-2369-OUT,B-6886-OUT;n:type:ShaderForge.SFN_Add,id:7533,x:36600,y:29857,varname:node_7533,prsc:2|A-6142-OUT,B-2135-OUT;n:type:ShaderForge.SFN_Vector2,id:5187,x:34250,y:28416,varname:node_5187,prsc:2,v1:0.333,v2:0.666;n:type:ShaderForge.SFN_Vector2,id:2135,x:36600,y:29987,varname:node_2135,prsc:2,v1:0.333,v2:0.666;n:type:ShaderForge.SFN_Vector1,id:6886,x:36379,y:30003,varname:node_6886,prsc:2,v1:0.666;n:type:ShaderForge.SFN_Panner,id:5346,x:36622,y:29703,varname:node_5346,prsc:2,spu:-1,spv:1|UVIN-7533-OUT,DIST-9023-OUT;n:type:ShaderForge.SFN_Panner,id:8859,x:36622,y:29551,varname:node_8859,prsc:2,spu:1,spv:-1|UVIN-2369-OUT,DIST-9023-OUT;n:type:ShaderForge.SFN_Time,id:9786,x:36184,y:30361,varname:node_9786,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:1161,x:36184,y:30260,ptovrint:False,ptlb:Foam Speed,ptin:_FoamSpeed,varname:node_1161,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:9023,x:36404,y:30256,varname:node_9023,prsc:2|A-1161-OUT,B-9786-TSL;n:type:ShaderForge.SFN_Multiply,id:4593,x:37363,y:29411,varname:node_4593,prsc:2|A-2758-RGB,B-5223-RGB,C-5827-OUT;n:type:ShaderForge.SFN_OneMinus,id:2936,x:37341,y:29693,varname:node_2936,prsc:2|IN-2784-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4035,x:36936,y:29626,ptovrint:False,ptlb:Foam Intensity,ptin:_FoamIntensity,varname:node_4035,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:7631,x:36936,y:29733,ptovrint:False,ptlb:Foam Distance,ptin:_FoamDistance,varname:node_7631,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_DepthBlend,id:2784,x:37122,y:29733,varname:node_2784,prsc:2|DIST-7631-OUT;n:type:ShaderForge.SFN_OneMinus,id:541,x:35028,y:30185,varname:node_541,prsc:2|IN-221-OUT;n:type:ShaderForge.SFN_Slider,id:5192,x:35273,y:30376,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_5192,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8988418,max:1;n:type:ShaderForge.SFN_Tex2d,id:8392,x:35758,y:31196,ptovrint:False,ptlb:Displacement Mask,ptin:_DisplacementMask,varname:node_8392,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:306f7b8087773cb42a665b9c5eeff8f1,ntxv:0,isnm:False|UVIN-7065-OUT;n:type:ShaderForge.SFN_Tex2d,id:881,x:36131,y:31359,ptovrint:False,ptlb:Displacement Tile,ptin:_DisplacementTile,varname:node_881,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1494fa713d330b84b9ce081df7b5c0c0,ntxv:0,isnm:False|UVIN-6159-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:4070,x:35728,y:31457,varname:node_4070,prsc:2,uv:0;n:type:ShaderForge.SFN_RemapRange,id:5068,x:35728,y:31617,varname:node_5068,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-4070-UVOUT;n:type:ShaderForge.SFN_Length,id:1302,x:35936,y:31617,varname:node_1302,prsc:2|IN-5068-OUT;n:type:ShaderForge.SFN_Append,id:2223,x:36131,y:31617,varname:node_2223,prsc:2|A-1302-OUT,B-1302-OUT;n:type:ShaderForge.SFN_Multiply,id:5548,x:36345,y:31617,varname:node_5548,prsc:2|A-2223-OUT,B-4317-OUT,C-1270-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4317,x:36025,y:31846,ptovrint:False,ptlb:Waves Tile Scale,ptin:_WavesTileScale,varname:node_4317,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:6;n:type:ShaderForge.SFN_ComponentMask,id:1270,x:36468,y:31208,varname:node_1270,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2371-OUT;n:type:ShaderForge.SFN_TexCoord,id:5180,x:35511,y:31077,varname:node_5180,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:7065,x:35538,y:31261,varname:node_7065,prsc:2|A-5180-UVOUT,B-3176-OUT;n:type:ShaderForge.SFN_Vector1,id:3176,x:35538,y:31392,varname:node_3176,prsc:2,v1:-1;n:type:ShaderForge.SFN_Slider,id:8904,x:35679,y:31392,ptovrint:False,ptlb:Mask Intensity,ptin:_MaskIntensity,varname:node_8904,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:2371,x:35960,y:31213,varname:node_2371,prsc:2|A-8392-RGB,B-8904-OUT;n:type:ShaderForge.SFN_Panner,id:6159,x:36611,y:31417,varname:node_6159,prsc:2,spu:1,spv:1|UVIN-5548-OUT,DIST-7153-OUT;n:type:ShaderForge.SFN_Time,id:9767,x:36566,y:31752,varname:node_9767,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:6101,x:36566,y:31666,ptovrint:False,ptlb:Displacement Speed,ptin:_DisplacementSpeed,varname:node_6101,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:7153,x:36818,y:31701,varname:node_7153,prsc:2|A-6101-OUT,B-9767-TSL,C-8117-OUT;n:type:ShaderForge.SFN_ToggleProperty,id:1900,x:36751,y:31916,ptovrint:False,ptlb:Inverse Waves Direction,ptin:_InverseWavesDirection,varname:node_1900,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_RemapRange,id:8117,x:36931,y:31904,varname:node_8117,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-1900-OUT;n:type:ShaderForge.SFN_Multiply,id:3816,x:36556,y:31093,varname:node_3816,prsc:2|A-881-R,B-6643-OUT,C-9490-OUT;n:type:ShaderForge.SFN_Vector3,id:6643,x:36556,y:30944,varname:node_6643,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_ValueProperty,id:9490,x:36236,y:31160,ptovrint:False,ptlb:Waves Amplitude,ptin:_WavesAmplitude,varname:node_9490,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Desaturate,id:9691,x:36984,y:30434,varname:node_9691,prsc:2|COL-3816-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8585,x:37243,y:30501,ptovrint:False,ptlb:Foam Waves Intensity,ptin:_FoamWavesIntensity,varname:node_8585,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:8122,x:37739,y:29601,varname:node_8122,prsc:2|A-4593-OUT,B-2936-OUT,C-4035-OUT;n:type:ShaderForge.SFN_Multiply,id:4527,x:36536,y:30466,varname:node_4527,prsc:2|A-4699-OUT,B-8585-OUT,C-4593-OUT;n:type:ShaderForge.SFN_Clamp01,id:4699,x:36970,y:30257,varname:node_4699,prsc:2|IN-9691-OUT;n:type:ShaderForge.SFN_Slider,id:8798,x:35482,y:30956,ptovrint:False,ptlb:Water Opacity,ptin:_WaterOpacity,varname:node_8798,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:1276,x:35695,y:30796,varname:node_1276,prsc:2|A-1033-OUT,B-8798-OUT;n:type:ShaderForge.SFN_Clamp01,id:2451,x:35886,y:30814,varname:node_2451,prsc:2|IN-1276-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:1131,x:36262,y:30521,varname:node_1131,prsc:2,min:0,max:10|IN-4527-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:221,x:37997,y:29602,varname:node_221,prsc:2,min:0,max:1|IN-8122-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:8025,x:36006,y:30420,varname:node_8025,prsc:2,min:0.1,max:1|IN-5192-OUT;n:type:ShaderForge.SFN_NormalVector,id:658,x:34948,y:31268,prsc:2,pt:False;n:type:ShaderForge.SFN_ComponentMask,id:9540,x:35181,y:31188,varname:node_9540,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-658-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:9509,x:35251,y:31387,varname:node_9509,prsc:2,min:0.3,max:1|IN-9540-OUT;n:type:ShaderForge.SFN_Multiply,id:3155,x:35944,y:31006,varname:node_3155,prsc:2|A-2451-OUT,B-9509-OUT;n:type:ShaderForge.SFN_OneMinus,id:9336,x:34777,y:31093,varname:node_9336,prsc:2|IN-9540-OUT;n:type:ShaderForge.SFN_RemapRange,id:15,x:34958,y:31090,varname:node_15,prsc:2,frmn:0,frmx:1,tomn:1,tomx:2|IN-9336-OUT;n:type:ShaderForge.SFN_Add,id:3830,x:37529,y:30898,varname:node_3830,prsc:2|A-1055-OUT,B-221-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:113,x:37595,y:28872,varname:node_113,prsc:2;n:type:ShaderForge.SFN_LightColor,id:3682,x:37595,y:28742,varname:node_3682,prsc:2;n:type:ShaderForge.SFN_LightVector,id:9780,x:37595,y:29019,varname:node_9780,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9841,x:37595,y:29160,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:7425,x:37831,y:29100,varname:node_7425,prsc:2,dt:1|A-9780-OUT,B-9841-OUT;n:type:ShaderForge.SFN_Multiply,id:5827,x:37930,y:28741,varname:node_5827,prsc:2|A-3682-RGB,B-113-OUT,C-7425-OUT;proporder:9873-9757-1541-8584-6538-5192-2533-1891-5389-9511-8798-149-5807-1999-3209-8163-1161-4035-8585-7631-8392-881-4317-8904-6101-1900-9490;pass:END;sub:END;*/

Shader "DCG/Water Shader/Ocean - Reflection Probes" {
    Properties {
        _Normal ("Normal", 2D) = "bump" {}
        _WaterColor ("Water Color", Color) = (0.2941176,1,1,1)
        _Density ("Density", Float ) = 1
        _FadeLevel ("Fade Level", Float ) = 4
        _Specularity ("Specularity", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8988418
        _Fresnel ("Fresnel", Float ) = 3
        _Scale ("Scale", Float ) = 1
        _NormalIntensity ("Normal Intensity", Range(0, 2)) = 2
        _WaterSpeed ("Water Speed", Float ) = 1
        _WaterOpacity ("Water Opacity", Range(0, 1)) = 1
        _ShoreOpacity ("Shore Opacity", Float ) = 1
        _ShoreTransparency ("Shore Transparency", Float ) = 6
        _RefractionIntensity ("Refraction Intensity", Range(0, 1)) = 0
        _Foam ("Foam", 2D) = "white" {}
        _FoamScale ("Foam Scale", Float ) = 1
        _FoamSpeed ("Foam Speed", Float ) = 1
        _FoamIntensity ("Foam Intensity", Float ) = 1
        _FoamWavesIntensity ("Foam Waves Intensity", Float ) = 1
        _FoamDistance ("Foam Distance", Float ) = 2
        _DisplacementMask ("Displacement Mask", 2D) = "white" {}
        _DisplacementTile ("Displacement Tile", 2D) = "white" {}
        _WavesTileScale ("Waves Tile Scale", Float ) = 6
        _MaskIntensity ("Mask Intensity", Range(0, 1)) = 1
        _DisplacementSpeed ("Displacement Speed", Float ) = 1
        [MaterialToggle] _InverseWavesDirection ("Inverse Waves Direction", Float ) = 0
        _WavesAmplitude ("Waves Amplitude", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
         //   Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma target 3.0
            #pragma glsl
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float4 _WaterColor;
            uniform float _Density;
            uniform float _FadeLevel;
            uniform float _Specularity;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Scale;
            uniform float _WaterSpeed;
            uniform float _NormalIntensity;
            uniform float _Fresnel;
            uniform float _ShoreOpacity;
            uniform float _ShoreTransparency;
            uniform float _RefractionIntensity;
            uniform sampler2D _Foam; uniform float4 _Foam_ST;
            uniform float _FoamScale;
            uniform float _FoamSpeed;
            uniform float _FoamIntensity;
            uniform float _FoamDistance;
            uniform float _Gloss;
            uniform sampler2D _DisplacementMask; uniform float4 _DisplacementMask_ST;
            uniform sampler2D _DisplacementTile; uniform float4 _DisplacementTile_ST;
            uniform float _WavesTileScale;
            uniform float _MaskIntensity;
            uniform float _DisplacementSpeed;
            uniform fixed _InverseWavesDirection;
            uniform float _WavesAmplitude;
            uniform float _FoamWavesIntensity;
            uniform float _WaterOpacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float4 projPos : TEXCOORD6;
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                float4 node_9767 = _Time + _TimeEditor;
                float node_1302 = length((o.uv0*2.0+-1.0));
                float2 node_7065 = (o.uv0*(-1.0));
                float4 _DisplacementMask_var = tex2Dlod(_DisplacementMask,float4(TRANSFORM_TEX(node_7065, _DisplacementMask),0.0,0));
                float2 node_6159 = ((float2(node_1302,node_1302)*_WavesTileScale*(_DisplacementMask_var.rgb*_MaskIntensity).rg)+(_DisplacementSpeed*node_9767.r*(_InverseWavesDirection*2.0+-1.0))*float2(1,1));
                float4 _DisplacementTile_var = tex2Dlod(_DisplacementTile,float4(TRANSFORM_TEX(node_6159, _DisplacementTile),0.0,0));
                float3 node_3816 = (_DisplacementTile_var.r*float3(0,1,0)*_WavesAmplitude);
                v.vertex.xyz += node_3816;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_7509 = _Time + _TimeEditor;
                float node_4302 = (_WaterSpeed*node_7509.r);
                float2 node_7657 = (float2(objScale.r,objScale.b)*i.uv0*_Scale);
                float2 node_5018 = ((float2(0.333,0.666)+(0.666*node_7657))+node_4302*float2(-1,1));
                float3 node_1516 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_5018, _Normal)));
                float2 node_9281 = (node_7657+node_4302*float2(1,-1));
                float3 node_5988 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_9281, _Normal)));
                float3 node_3745 = lerp(float3(0,0,1),float3((node_1516.r+node_5988.r),(node_1516.g+node_5988.g),1.0),_NormalIntensity);
                float3 normalLocal = node_3745;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float node_9540 = i.normalDir.g;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_3745.rg*(_RefractionIntensity*0.1+0.0)*((1.0 - node_9540)*1.0+1.0));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = clamp(_Gloss,0.1,1);
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 node_9767 = _Time + _TimeEditor;
                float node_1302 = length((i.uv0*2.0+-1.0));
                float2 node_7065 = (i.uv0*(-1.0));
                float4 _DisplacementMask_var = tex2D(_DisplacementMask,TRANSFORM_TEX(node_7065, _DisplacementMask));
                float2 node_6159 = ((float2(node_1302,node_1302)*_WavesTileScale*(_DisplacementMask_var.rgb*_MaskIntensity).rg)+(_DisplacementSpeed*node_9767.r*(_InverseWavesDirection*2.0+-1.0))*float2(1,1));
                float4 _DisplacementTile_var = tex2D(_DisplacementTile,TRANSFORM_TEX(node_6159, _DisplacementTile));
                float3 node_3816 = (_DisplacementTile_var.r*float3(0,1,0)*_WavesAmplitude);
                float4 node_9786 = _Time + _TimeEditor;
                float node_9023 = (_FoamSpeed*node_9786.r);
                float2 node_2369 = (float2(objScale.r,objScale.b)*i.uv0*_FoamScale);
                float2 node_8859 = (node_2369+node_9023*float2(1,-1));
                float4 node_2758 = tex2D(_Foam,TRANSFORM_TEX(node_8859, _Foam));
                float2 node_5346 = (((node_2369*0.666)+float2(0.333,0.666))+node_9023*float2(-1,1));
                float4 node_5223 = tex2D(_Foam,TRANSFORM_TEX(node_5346, _Foam));
                float3 node_4593 = (node_2758.rgb*node_5223.rgb*(_LightColor0.rgb*attenuation*max(0,dot(lightDirection,i.normalDir))));
                float3 node_1131 = clamp((saturate(dot(node_3816,float3(0.3,0.59,0.11)))*_FoamWavesIntensity*node_4593),0,10);
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float node_2311 = (_Specularity*pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel));
                float3 specularColor = float3(node_2311,node_2311,node_2311);
                float specularMonochrome = max( max(specularColor.r, specularColor.g), specularColor.b);
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular + node_1131);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float node_5971 = 1.0;
                float3 w = float3(node_5971,node_5971,node_5971)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_5971,node_5971,node_5971);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                NdotLWrap = max(float3(0,0,0), NdotLWrap);
                float3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*pow((1.00001-NdotLWrap), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL))*(0.5-max(w.r,max(w.g,w.b))*0.5) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 node_221 = clamp((node_4593*(1.0 - saturate((sceneZ-partZ)/_FoamDistance))*_FoamIntensity),0,1);
                indirectDiffuse += node_221; // Diffuse Ambient Light
                float node_9504 = (-0.1);
                float node_3478 = 1.0;
                float3 diffuseColor = ((saturate((pow(saturate((node_3478 + ( (((sceneZ-partZ)*_Density) - node_9504) * (0.0 - node_3478) ) / ((_WaterColor.rgb*6.28318530718) - node_9504))),_FadeLevel)+(1.0 - saturate((sceneZ-partZ)/_ShoreTransparency))))*sceneColor.rgb)+node_221);
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = node_1131;
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,(saturate((saturate((sceneZ-partZ)/_ShoreOpacity)*_WaterOpacity))*clamp(node_9540,0.3,1))),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma target 3.0
            #pragma glsl
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float4 _WaterColor;
            uniform float _Density;
            uniform float _FadeLevel;
            uniform float _Specularity;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Scale;
            uniform float _WaterSpeed;
            uniform float _NormalIntensity;
            uniform float _Fresnel;
            uniform float _ShoreOpacity;
            uniform float _ShoreTransparency;
            uniform float _RefractionIntensity;
            uniform sampler2D _Foam; uniform float4 _Foam_ST;
            uniform float _FoamScale;
            uniform float _FoamSpeed;
            uniform float _FoamIntensity;
            uniform float _FoamDistance;
            uniform float _Gloss;
            uniform sampler2D _DisplacementMask; uniform float4 _DisplacementMask_ST;
            uniform sampler2D _DisplacementTile; uniform float4 _DisplacementTile_ST;
            uniform float _WavesTileScale;
            uniform float _MaskIntensity;
            uniform float _DisplacementSpeed;
            uniform fixed _InverseWavesDirection;
            uniform float _WavesAmplitude;
            uniform float _FoamWavesIntensity;
            uniform float _WaterOpacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float4 projPos : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                float4 node_9767 = _Time + _TimeEditor;
                float node_1302 = length((o.uv0*2.0+-1.0));
                float2 node_7065 = (o.uv0*(-1.0));
                float4 _DisplacementMask_var = tex2Dlod(_DisplacementMask,float4(TRANSFORM_TEX(node_7065, _DisplacementMask),0.0,0));
                float2 node_6159 = ((float2(node_1302,node_1302)*_WavesTileScale*(_DisplacementMask_var.rgb*_MaskIntensity).rg)+(_DisplacementSpeed*node_9767.r*(_InverseWavesDirection*2.0+-1.0))*float2(1,1));
                float4 _DisplacementTile_var = tex2Dlod(_DisplacementTile,float4(TRANSFORM_TEX(node_6159, _DisplacementTile),0.0,0));
                float3 node_3816 = (_DisplacementTile_var.r*float3(0,1,0)*_WavesAmplitude);
                v.vertex.xyz += node_3816;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_7509 = _Time + _TimeEditor;
                float node_4302 = (_WaterSpeed*node_7509.r);
                float2 node_7657 = (float2(objScale.r,objScale.b)*i.uv0*_Scale);
                float2 node_5018 = ((float2(0.333,0.666)+(0.666*node_7657))+node_4302*float2(-1,1));
                float3 node_1516 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_5018, _Normal)));
                float2 node_9281 = (node_7657+node_4302*float2(1,-1));
                float3 node_5988 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_9281, _Normal)));
                float3 node_3745 = lerp(float3(0,0,1),float3((node_1516.r+node_5988.r),(node_1516.g+node_5988.g),1.0),_NormalIntensity);
                float3 normalLocal = node_3745;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float node_9540 = i.normalDir.g;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_3745.rg*(_RefractionIntensity*0.1+0.0)*((1.0 - node_9540)*1.0+1.0));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = clamp(_Gloss,0.1,1);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float node_2311 = (_Specularity*pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel));
                float3 specularColor = float3(node_2311,node_2311,node_2311);
                float specularMonochrome = max( max(specularColor.r, specularColor.g), specularColor.b);
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float node_5971 = 1.0;
                float3 w = float3(node_5971,node_5971,node_5971)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_5971,node_5971,node_5971);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                NdotLWrap = max(float3(0,0,0), NdotLWrap);
                float3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*pow((1.00001-NdotLWrap), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL))*(0.5-max(w.r,max(w.g,w.b))*0.5) * attenColor;
                float node_9504 = (-0.1);
                float node_3478 = 1.0;
                float4 node_9786 = _Time + _TimeEditor;
                float node_9023 = (_FoamSpeed*node_9786.r);
                float2 node_2369 = (float2(objScale.r,objScale.b)*i.uv0*_FoamScale);
                float2 node_8859 = (node_2369+node_9023*float2(1,-1));
                float4 node_2758 = tex2D(_Foam,TRANSFORM_TEX(node_8859, _Foam));
                float2 node_5346 = (((node_2369*0.666)+float2(0.333,0.666))+node_9023*float2(-1,1));
                float4 node_5223 = tex2D(_Foam,TRANSFORM_TEX(node_5346, _Foam));
                float3 node_4593 = (node_2758.rgb*node_5223.rgb*(_LightColor0.rgb*attenuation*max(0,dot(lightDirection,i.normalDir))));
                float3 node_221 = clamp((node_4593*(1.0 - saturate((sceneZ-partZ)/_FoamDistance))*_FoamIntensity),0,1);
                float3 diffuseColor = ((saturate((pow(saturate((node_3478 + ( (((sceneZ-partZ)*_Density) - node_9504) * (0.0 - node_3478) ) / ((_WaterColor.rgb*6.28318530718) - node_9504))),_FadeLevel)+(1.0 - saturate((sceneZ-partZ)/_ShoreTransparency))))*sceneColor.rgb)+node_221);
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * (saturate((saturate((sceneZ-partZ)/_ShoreOpacity)*_WaterOpacity))*clamp(node_9540,0.3,1)),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _DisplacementMask; uniform float4 _DisplacementMask_ST;
            uniform sampler2D _DisplacementTile; uniform float4 _DisplacementTile_ST;
            uniform float _WavesTileScale;
            uniform float _MaskIntensity;
            uniform float _DisplacementSpeed;
            uniform fixed _InverseWavesDirection;
            uniform float _WavesAmplitude;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                float4 node_9767 = _Time + _TimeEditor;
                float node_1302 = length((o.uv0*2.0+-1.0));
                float2 node_7065 = (o.uv0*(-1.0));
                float4 _DisplacementMask_var = tex2Dlod(_DisplacementMask,float4(TRANSFORM_TEX(node_7065, _DisplacementMask),0.0,0));
                float2 node_6159 = ((float2(node_1302,node_1302)*_WavesTileScale*(_DisplacementMask_var.rgb*_MaskIntensity).rg)+(_DisplacementSpeed*node_9767.r*(_InverseWavesDirection*2.0+-1.0))*float2(1,1));
                float4 _DisplacementTile_var = tex2Dlod(_DisplacementTile,float4(TRANSFORM_TEX(node_6159, _DisplacementTile),0.0,0));
                float3 node_3816 = (_DisplacementTile_var.r*float3(0,1,0)*_WavesAmplitude);
                v.vertex.xyz += node_3816;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    //CustomEditor "ShaderForgeMaterialInspector"

}
