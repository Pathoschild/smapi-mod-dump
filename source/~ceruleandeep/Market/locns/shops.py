import sys

import numpy as np
from jinja2 import Environment, PackageLoader, select_autoescape
env = Environment(
    loader=PackageLoader("shops"),
    autoescape=select_autoescape()
)

joseph = "Elaho.JosephsSeedShopDisplayCP"
paisley = "Elaho.PaisleysBridalBoutiqueCP"

template = env.get_template("patch.json")


class Gen:
    def render(self):
        variants = "Bismarck, BurntOrange, Plain, Leaves, Purple, Spots, TwoTone".split(", ")

        layouts = {
            "1 Shop": [1],
            "3 Shops": [1, 7, 8],
            "5 Shops": [1, 5, 6, 7, 8],
            "6 shops": [1, 2, 6, 7, 8, 9],
            "7 Shops": [1, 2, 5, 6, 7, 8, 9],
            "9 Shops": range(1, 10),
        }

        exclude_joseph = [2, 7]
        exclude_paisley = [5]

        in_layout = {}
        for layout, positions in layouts.items():
            for position in positions:
                if position not in in_layout:
                    in_layout[position] = []
                in_layout[position].append(layout)

        shop1x = 27
        shop1y = 58

        shops = [np.array([22, 58])]
        shops.append(shops[-1] + np.array([5, 0]))
        shops.append(shops[-1] + np.array([5, 0]))
        shops.append(shops[-1] + np.array([5, 0]))
        shops.append(shops[-1] + np.array([5, 0]))

        shops.append(shops[1] + np.array([-5, 5]))
        shops.append(shops[-1] + np.array([5, 0]))
        shops.append(shops[-1] + np.array([5, 0]))

        shops.append(shops[-1] + np.array([-5, 5]))
        shops.append(shops[-1] + np.array([5, 0]))

        for i, shop in enumerate(shops):
            if i == 0:
                continue
            variant = variants[i%len(variants)]
            layouts = ", ".join(in_layout[i])

            when = ['"HasMod": "ceruleandeep.MarketDay"']
            if i in exclude_joseph:
                when.append(f'HasMod |contains={joseph}": false')
            if i in exclude_paisley:
                when.append(f'HasMod |contains={paisley}": false')

            hasmod = ", ".join(when)
            # json = template.replace("%i", str(i))\
            #     .replace("%x", str(shop[0])).replace("%y", str(shop[1])).replace("%sx", str(shop[0] + 1))\
            #     .replace("%v", variant).replace("%l", layouts)
            print(template.render(shopnum=i, variant=variant, layouts=layouts, hasmod=hasmod, x=shop[0], y=shop[1], shopx=shop[0]+1))

if __name__ == "__main__":
    Gen().render()