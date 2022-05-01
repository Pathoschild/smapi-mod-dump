import dragonfly as df
import title_menu, menu_utils, server, df_utils, approximate_matching, items, server
from game_menu import game_menu

skills = {
    k: i
    for (i, k) in enumerate(
        (
            "farming",
            "mining",
            "foraging",
            "fishing",
            "combat",
        )
    )
}


def get_inventory_page(menu):
    page = game_menu.get_page_by_name(menu, "skillsPage")
    return page


async def focus_item_dictation(page, text):
    cmp = approximate_matching.match_component(text, page["specialItems"], "hoverText")
    await menu_utils.focus_component(cmp)


async def focus_skill(page, index):
    cmp = page["skillAreas"][index]
    await menu_utils.focus_component(cmp)


mapping = {
    "<skills>": df_utils.async_action(focus_skill, "skills"),
    "<dictation>": df_utils.async_action(focus_item_dictation, "dictation"),
}


def load_grammar():
    extras = [df_utils.dictation_rule(), df.Choice("skills", skills)]
    grammar = menu_utils.build_menu_grammar(mapping, get_inventory_page, extras=extras)
    grammar.load()


# {
#     "xPositionOnScreen": 413,
#     "yPositionOnScreen": 140,
#     "upperRightCloseButton": null,
#     "containsMouse": true,
#     "classType": "SkillsPage",
#     "menuType": "skillsPage",
#     "skillAreas": [
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 625, "y": 264, "width": 148, "height": 36},
#             "center": [699.0, 282.0],
#             "name": "0",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "+9 Hoe Efficiency\r\n+9 Water Can Efficiency",
#         },
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 625, "y": 320, "width": 148, "height": 36},
#             "center": [699.0, 338.0],
#             "name": "3",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "+8 Pickaxe Efficiency",
#         },
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 625, "y": 376, "width": 148, "height": 36},
#             "center": [699.0, 394.0],
#             "name": "2",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "+8 Axe Efficiency",
#         },
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 625, "y": 432, "width": 148, "height": 36},
#             "center": [699.0, 450.0],
#             "name": "1",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "+7 Fishing Rod Efficiency",
#         },
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 625, "y": 488, "width": 148, "height": 36},
#             "center": [699.0, 506.0],
#             "name": "4",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "+30 Health",
#         },
#     ],
#     "skillBars": [
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 941, "y": 264, "width": 56, "height": 36},
#             "center": [969.0, 282.0],
#             "name": "1",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "Crops worth 10% more.",
#         },
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 941, "y": 320, "width": 56, "height": 36},
#             "center": [969.0, 338.0],
#             "name": "19",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "Chance for gems to appear in pairs.",
#         },
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 941, "y": 376, "width": 56, "height": 36},
#             "center": [969.0, 394.0],
#             "name": "12",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "Trees drop 25% more wood.",
#         },
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 941, "y": 432, "width": 56, "height": 36},
#             "center": [969.0, 450.0],
#             "name": "6",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "Fish worth 25% more.",
#         },
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 941, "y": 488, "width": 56, "height": 36},
#             "center": [969.0, 506.0],
#             "name": "24",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "All attacks deal 10% more damage.",
#         },
#     ],
#     "specialItems": [
#         null,
#         null,
#         null,
#         null,
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 753, "y": 656, "width": 64, "height": 64},
#             "center": [785.0, 688.0],
#             "name": "",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "Skull Key",
#         },
#         {
#             "type": "clickableComponent",
#             "bounds": {"x": 814, "y": 656, "width": 64, "height": 64},
#             "center": [846.0, 688.0],
#             "name": "",
#             "containsMouse": false,
#             "visible": true,
#             "hoverText": "Magnifying Glass",
#         },
#         null,
#         null,
#         null,
#         null,
#         null,
#     ],
# }
