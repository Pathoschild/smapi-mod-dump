namespace SpriteMaster.Harmonize.Patches.PSpriteBatch.Patch {
	internal static class StableSort {
#if STABLE_SORT
		private static class Comparer {
			private static FieldInfo TextureField;
			private static FieldInfo SpriteField;
			private static MethodInfo TextureComparer;
			private static MethodInfo SpriteGetter;
			private static FieldInfo GetSource;
			private static FieldInfo GetOrigin;

			static Comparer () {
				TextureField = typeof(SpriteBatch).GetField("spriteTextures", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				SpriteField = typeof(SpriteBatch).GetField("spriteQueue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				TextureComparer = typeof(Texture).GetMethod("CompareTo", BindingFlags.Instance | BindingFlags.NonPublic);
				SpriteGetter = SpriteField.FieldType.GetMethod("GetValue", new Type[] { typeof(int) });
				GetSource = SpriteField.FieldType.GetElementType().GetField("Source", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				GetOrigin = SpriteField.FieldType.GetElementType().GetField("Origin", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}

			static int Compare (in Vector2 lhs, in Vector2 rhs) {
				if (lhs.X < rhs.X) {
					return -1;
				}
				else if (lhs.X > rhs.X) {
					return 1;
				}

				if (lhs.Y < rhs.Y) {
					return -1;
				}
				else if (lhs.Y > rhs.Y) {
					return 1;
				}

				return 0;
			}

			static int Compare (in Vector4 lhs, in Vector4 rhs) {
				int comparison = Compare(new Vector2(lhs.X, lhs.Y), new Vector2(rhs.X, rhs.Y));
				if (comparison != 0) {
					return comparison;
				}

				return Compare(new Vector2(lhs.Z, lhs.W), new Vector2(rhs.Z, rhs.W));
			}

			[HarmonyPatch(typeof(SpriteBatch), "BackToFrontComparer", "Compare", isChild: true, HarmonyPatch.Fixation.Postfix, HarmonyExt.PriorityLevel.Last)]
			internal static void BFComparer (object __instance, ref int __result, SpriteBatch ___parent, int x, int y) {
				if (__result != 0)
					return;

				var textures = (Texture[])TextureField.GetValue(___parent);

				var texture1 = textures[x];
				var texture2 = textures[y];

				var comparison = (int)TextureComparer.Invoke(texture1, new object[] { texture2 });
				if (comparison != 0) {
					__result = comparison;
					return;
				}

				var spriteQueue = SpriteField.GetValue(___parent);

				var sprite1 = SpriteGetter.Invoke(spriteQueue, new object[] { x });
				var sprite2 = SpriteGetter.Invoke(spriteQueue, new object[] { y });

				var source1 = (Vector4)GetSource.GetValue(sprite1);
				var source2 = (Vector4)GetSource.GetValue(sprite2);

				comparison = Compare(source1, source2);
				if (comparison != 0) {
					__result = comparison;
					return;
				}

				var origin1 = (Vector2)GetOrigin.GetValue(sprite1);
				var origin2 = (Vector2)GetOrigin.GetValue(sprite2);

				comparison = Compare(origin1, origin2);
				if (comparison != 0) {
					__result = comparison;
					return;
				}

				__result = y.CompareTo(x);
			}

			[HarmonyPatch(typeof(SpriteBatch), "FrontToBackComparer", "Compare", isChild: true, HarmonyPatch.Fixation.Postfix, HarmonyExt.PriorityLevel.Last)]
			internal static void FBComparer (object __instance, ref int __result, SpriteBatch ___parent, int x, int y) {
				if (__result != 0)
					return;

				var textures = (Texture[])TextureField.GetValue(___parent);

				var texture1 = textures[y];
				var texture2 = textures[x];

				var comparison = (int)TextureComparer.Invoke(texture1, new object[] { texture2 });
				if (comparison != 0) {
					__result = comparison;
					return;
				}

				var spriteQueue = SpriteField.GetValue(___parent);

				var sprite1 = SpriteGetter.Invoke(spriteQueue, new object[] { y });
				var sprite2 = SpriteGetter.Invoke(spriteQueue, new object[] { x });

				var source1 = (Vector4)GetSource.GetValue(sprite1);
				var source2 = (Vector4)GetSource.GetValue(sprite2);

				comparison = Compare(source1, source2);
				if (comparison != 0) {
					__result = comparison;
					return;
				}

				var origin1 = (Vector2)GetOrigin.GetValue(sprite1);
				var origin2 = (Vector2)GetOrigin.GetValue(sprite2);

				comparison = Compare(origin1, origin2);
				if (comparison != 0) {
					__result = comparison;
					return;
				}

				__result = x.CompareTo(y);
			}
		}
#endif
	}
}
