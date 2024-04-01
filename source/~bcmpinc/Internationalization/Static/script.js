/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

// Internationalization : Stardew Valley Mod Translation Tool
const el = {};
const info = {};
const iso639_1 = {};
const language = {};

// Translation json parser.
const magic = new RegExp([
	/(?:(?<key1>[_a-z][_a-z0-9]*)|"(?<key2>.*?)(?<!\\(?:\\\\)*)")(?<colon>\s*:\s*)(?<value>".*?(?<!\\(?:\\\\)*)")/, // entry
	/\/\/(?<sc>.*)/,      // Single line comment
	/\/\*(?<mc>[^]*?)\*\//, // Multiline comment
].map((x)=>x.source).join('|'), "dgiu");

// Request mod list
Promise.all([
	fetch("/info").then(as_json).then((res) => Object.assign(info, res)),
	fetch("/static/iso639-1.json").then(as_json).then((res) => Object.assign(iso639_1, res)),
	fetch("/lang/bcmpinc.Internationalization/current").then(as_json).then(async (res) => {
		Object.assign(language, res);
		await content_loaded;
		for (const element of $('//*[@data-i18n]')) {
			const value = res[element.dataset['i18n']];
			if (value) {
				element.replaceChildren(text(value));
			}
		}
	}),
]).then(ready).catch(show_error);

/** Returns an array containing all elements matched by the given XPath expression. */
function $(a, root) {
	const res = document.evaluate(a,root ?? document,null,XPathResult.ORDERED_NODE_SNAPSHOT_TYPE,null);
	return [...gen()];
	function* gen() {
		for (let i=0; i<res.snapshotLength; i++) yield res.snapshotItem(i);
	}
}

/** Create a new element */
function node(nodeType, pars){
	const e = document.createElement(nodeType);
	for (const p in pars) {
		if (p=="text") e.appendChild(text(pars[p]));
		else e.setAttribute(p,pars[p]);
	}
	return e;
}

/** Create a text node */
function text(content) {
	return document.createTextNode(content);
}

function is_ok(res) {if(!res.ok) throw res;}
function as_text(res) {is_ok(res); return res.text();}
function as_json(res) {is_ok(res); return res.json();}

/** Modify textarea to fit contents. Must be part of document to work. */
function textarea_fit(e) {
	e.style.height = "1lh";
	e.style.height = (e.scrollHeight-4)+"px";
}

/** 
 * Like Array.prototype.map, but for objects.
 * Takes an additional argument to determine sorting order.
 * Both cmp_prop and fn have the signature function(element, index, object).
 * @param cmp_prop function that returns a object used as sorting key.
 * @param fn function that is executed for each element.
 * @returns An array of the values returned by fn.
 */
function sort_and_map(object, cmp_prop, fn) {
	const keys = Object.getOwnPropertyNames(object);
	if (cmp_prop) keys.sort(compare_property((a) => cmp_prop(object[a],a,object)));
	return keys.map( (x) => fn(object[x],x,object) );
}

/** Returns a comparator for use in sort, that compares the values returned by fn. */
function compare_property(fn) {
	return (a,b) => {
		const aa = fn(a);
		const bb = fn(b);
		if (aa < bb) return -1;
		if (aa > bb) return  1;
		return 0;
	}
}


/** Returns the selected option for a given <select> element. */
function current_option(element) {
	return element.options[element.selectedIndex];
}

/** Causse a <select> to copy its css-class from the selected option. */
function copy_style_from_option(element) {
	if (element.target) element = element.target;
	element.className = current_option(element).className
}

/** Tries parsing a JSON encoded string. */
function json_try_parse(value) {
	try	{
		return [JSON.parse(value), false];
	} catch(err) {
		return [value, err];
	}
}

function get_locale_name(locale) {
	return iso639_1[locale] ?? ("[" + locale + "]");
}

/** Initialize the web app */
function ready() {
	// Map id to their html element
	for (const e of $("//*[@id]")) el[e.id.replaceAll("-","_")] = e;

	el.save.addEventListener('click', save);
	el.download.addEventListener('click', download);
	el.hide_error.addEventListener('click', ()=>el.error.parentNode.classList.add("hidden"));

	// Update textboxes to fit content on window resize.
	let resize_timer;
	let resize_pos;
	window.addEventListener("resize", () => {
		resize_pos = 0;
		if (!resize_timer) {
			resize_timer = setInterval(resize_textareas, 20);
		}
		function resize_textareas() {
			const array = $("//textarea");
			let count = 0;
			for (; resize_pos < array.length; resize_pos++) {
				textarea_fit(array[resize_pos]);
				if (++count >= 50) return;
			}
			clearInterval(resize_timer)
			resize_timer = null;
		}
	});
	
	// Populate mod picker.
	const mod_options = sort_and_map(info.mods, (x)=>x.name, (mod,id) => node("option", {value:id, text: mod.name}));
	el.mod.replaceChildren(...mod_options);
	el.mod.value = localStorage.getItem("modid"); // Select last mod
	el.mod.addEventListener('change', update_mod);
	el.mod.addEventListener('change', copy_style_from_option);

	// Populate locale picker.
	const locale_options = sort_and_map(info.locales, (_,id)=>get_locale_name(id), locale_option);
	el.locale.replaceChildren(...locale_options);
	el.locale.value = localStorage.getItem("locale") ?? info.current_locale; // Select previous locale
	el.locale.addEventListener('change', update_locale);
	el.locale.addEventListener('change', copy_style_from_option);
	
	// Configure current locale button
	el.current.replaceChildren(text(get_locale_name(info.current_locale)));
	el.current.addEventListener('click', function(){
		el.locale.value = info.current_locale;
		update_locale();
		copy_style_from_option(el.locale);
	});

	update_mod();
	
	function locale_option(entry, id) {
		const res = node("option", {value:id, text:get_locale_name(id)})
		if (entry.modname) {
			res.setAttribute("title", entry.modname);
		}
		return res;
	}
	
	function status(id) {
		const loc = info.current_locale;
	}
}

/** Load the mod's translation file into the editor */
function update_mod() {
	const modid = el.mod.value;
	localStorage.setItem("modid", modid);
	
	// Update what locale availability for selected mod
	for (const loc of $("./option", el.locale)) {
		set_translation_status(loc, modid, loc.value);
	}
	copy_style_from_option(el.locale);
	
	const text_new = fetch("/file/" + modid + "/default").then(as_text).then((text_new) => {
		// Generate the translation editor for this mod
		el.new.replaceChildren(...generate_editor(text_new));
		el.new.dataset.raw = text_new;
	}).then(
		// Load the selected locale
		update_locale
	).catch(show_error);
}

function* generate_editor(content, readonly) {
	let pos = 0;
	for (const m of content.matchAll(magic)) {
		const g = m.groups;
		if (g.sc) yield node("div", {'class': "comment", text: g.sc});
		if (g.mc) yield node("div", {'class': "comment", text: g.mc});
		if (g.key1 || g.key2) {
			const r = node("div", {'class': "entry"});
			const key = g.key1 ?? g.key2;
			const [value, error] = json_try_parse(g.value);
			r.replaceChildren(...generate_entry());
			yield r;

			function* generate_entry() {
				yield node("span", {'class': "key", text: key});
				let field_value;
				let field_text;
				if (readonly) {
					yield field_value = node("span", {'class': "default", "data-key": key}),
					yield field_text = node("textarea", {'class': "value", text: value, readonly:""});
					field_text.addEventListener('focus', (e)=>e.target.select());
				} else {
					yield node("span", {'class': "default", text: value});
					yield field_value = field_text = node("textarea", {'class': "value", "data-key": key, "data-position":m.indices.groups.value});
					field_text.addEventListener('input', (e)=>textarea_fit(e.target));
					field_text.addEventListener('change', (e)=>set_text(e.target.dataset.key, e.target.value));
				}
				if (error) {
					field_value.classList.add("error");
					field_value.setAttribute("title", error);
				}
				r.addEventListener('click', ()=>field_text.focus());
			}
		}
		pos = m.index + m[0].length;
	}
}

/** Load the selected locale into the editor. */
async function update_locale() {
	const modid = el.mod.value;
	const locale = el.locale.value;
	localStorage.setItem("locale", locale);

	// Load current translation from game
	fetch("/lang/" + el.mod.value + "/" + locale).then(as_json).catch((x)=>Promise.resolve({})).then(
	(lang) => {
		for (const e of $('.//*[@data-key]', el.new)) {
			e.replaceChildren(text(lang[e.dataset.key] ?? ""));
			textarea_fit(e);
		}
		const [lines,total] = update_progress();
		const actual_total = info.mods[modid].lines_total;
		if (total != actual_total) {
			show_error(language["error.total_lines"].replace("{{count}}", actual_total))
		}
	}).catch(show_error);
	
	// Generate old translation contents
	fetch("/file/" + modid + "/" + locale).then(as_text).catch((x)=>Promise.resolve("")).then(
	(text_old) => {
		el.old.replaceChildren(...generate_editor(text_old, true));
		for (const x of $(".//textarea", el.old)) textarea_fit(x);
		fetch("/lang/" + el.mod.value + "/default").then(as_json).then(
		(lang) => {
			if (!lang) return;
			for (const e of $('.//*[@data-key]', el.old)) {
				e.replaceChildren(text(lang[e.dataset.key] ?? ""));
			}
		}).catch(show_error);	
	}).catch(show_error);
	
	// Update what mods have the current locale available
	for (const mod of $("./option", el.mod)) {
		set_translation_status(mod, mod.value, locale);
	}
	copy_style_from_option(el.mod);
	
	// (De)activate save button.
	el.save.disabled = !is_modified(el.mod.value, el.locale.value);
}

/** Updates a text in game for the current mod & locale. */
function set_text(text_id, value) {
	const mod = el.mod.value;
	const locale = el.locale.value;
	const content = { method: "PUT", body: value };
	fetch("/lang/"+mod+"/"+locale+"/"+text_id, content).catch(show_error);
	mark_modified(mod, locale, true);
}

function set_translation_status(element, mod, locale) {
	const mod_info = info.mods[mod];
	const locale_info = mod_info.locales[locale];
	element.className = "";
	if (locale_info) {
		if (locale_info.modified) {
			element.classList.add("modified");
		}
		if (locale_info.lines_translated < mod_info.lines_total) {
			element.classList.add("partial");
		} else {
			element.classList.add("complete");
		}
	} else {
		element.classList.add("missing");
	}	
}

/** Sets the modified flag for the given mod & locale. */
function mark_modified(mod, locale, status) {
	const locales = info.mods[mod].locales;
	if (locales[locale] && locales[locale].modified == status) return; // Nothing changed.
	
	// Set the new status
	if (locales[locale]) {
		locales[locale].modified = status;
	} else {
		locales[locale] = {
			modified: status,
			lines_translated: 0
		};
	}

	el.save.disabled = !status;
	update_progress();
	
	// Update the selection boxes formatting.
	set_translation_status(current_option(el.mod), mod, locale);
	set_translation_status(current_option(el.locale), mod, locale);
	copy_style_from_option(el.mod);
	copy_style_from_option(el.locale);
}

function update_progress() {
	const locale_info = info.mods[el.mod.value].locales[el.locale.value];
	const elements = $('.//*[@data-key]', el.new);
	const total = elements.length;
	const lines = elements.filter((x)=>x.value).length;
	if (locale_info) {
		locale_info.lines_translated = lines;
	}
	el.progress.replaceChildren(text(lines + " / " + total));
	return [lines,total];
}

function is_modified(mod, locale) {
	const locales = info.mods[mod].locales;
	return locales[locale] && locales[locale].modified;
}

function download() {
	const data = generate_file()
	var properties = {type: 'application/octet-stream'}; // Specify the file's mime-type.
	file = new File(data, el.locale.value + ".json", properties);
	var url = URL.createObjectURL(file);
	window.open(url); // Needs to happen in a click event handler.
	URL.revokeObjectURL(url)
}

function save() {
	const data = generate_file().join("");
	const mod = el.mod.value;
	const locale = el.locale.value;
	const content = { method: "PUT", body: data };
	fetch("/file/"+mod+"/"+locale, content).then((res) => {
		if (res.ok) {
			mark_modified(mod, locale, false)
		} else {
			show_error(res.status + " " + res.statusText + "\n\n");
			res.text().then((res) => el.error.appendChild(text(res)));
		}
	}).catch(show_error);
}

function parse_position(pos) {
	const a = pos.split(",");
	return {begin: +a[0], end: +a[1]};
}

function generate_file() {
	const raw = el.new.dataset.raw;
	const result = [];
	const entries = $('.//*[@data-position]', el.new);
	entries.sort(compare_property((e) => parse_position(e.dataset.position).begin));
	let pos = 0;
	for (const e of entries) {
		const range = parse_position(e.dataset.position);
		result.push(raw.slice(pos, range.begin));
		result.push(JSON.stringify(e.value));
		pos = range.end;
	}
	result.push(raw.slice(pos));
	return result;
}

function show_error(e) {
	el.error.replaceChildren(text(e));
	if (e.stack) el.error.appendChild(text("\n\n"+e.stack));
	el.error.parentNode.classList.remove("hidden");
}
