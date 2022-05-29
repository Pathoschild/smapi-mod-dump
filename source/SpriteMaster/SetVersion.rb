#!ruby

require 'json'
require 'nokogiri'
require 'pathname'
require 'open3'
require 'socket'

module FileNames
	Manifest = 'manifest.json'
	AssemblySource = 'Assembly.cs'
end

module Paths
	Props = Pathname.getwd + 'SpriteMasterCommon.props'
	Version = Pathname.getwd + 'version'
end

def eputs(*msg)
	STDERR.puts(*msg)
	STDERR.flush
end

def fail(*msg, code: -1)
	eputs(*msg)
	exit code
end

class Project
	@Path = nil
	def initialize(path)
		@Path = Pathname.new(path).freeze
		self.freeze
	end
	
	def path = @Path
	alias_method :to_s, :path
	alias_method :to_str, :path
	def manifest = @Path + 'manifest.json'
	def assembly = @Path + 'Assembly.cs'
end

def search_projects_recursive(current, &block)
	if (current + FileNames::Manifest).file? && (current + FileNames::AssemblySource).file?
		yield Project.new(current)
		return
	end
	
	current.children.select(&:directory?).each { |child|
		next if child.basename.to_s[0] == '.'
		
		search_projects_recursive(child, &block)
	}
end

Projects = to_enum(:search_projects_recursive, Pathname.getwd).to_a

fail "No projects could be found" if Projects.empty?
fail "Props file '#{Paths::Props}' could not be found" unless Paths::Props.file?

TAB = '  '

def indent_puts(*msg, indent: 1)
	puts *msg.map { |s| "#{TAB * indent}#{s}"}
end

class String
	def blank? = self.empty?
end

class Object
	def blank = self.blank? ? nil : self
	def blank?
		return true if self.nil?
		return self.empty? if self.respond_to?(:empty?)
		return self.size == 0 if self.respond_to?(:size)
		return self.length == 0 if self.respond_to?(:length)
		return self.count == 0 if self.respond_to?(:count)
		return false
	end
end

class SemanticVersion
	attr_accessor :major, :minor, :patch, :build
	attr_accessor :tag, :tag_version

	private
	def append_tag(string)
		unless @tag.nil? && @tag_version.nil?
			string += '-'
			string += @tag.to_s unless @tag.nil?
			unless @tag_version.nil?
				string += '.' unless @tag.nil?
				string += @tag_version.to_s
			end
		end
		return string
	end

	public
	def initialize(string: nil, major: nil, minor: nil, patch: nil, build: nil, tag: nil, tag_version: nil)
		unless string.nil?
			version_string, tag_string = string.split('-', 2)

			unless version_string.blank?
				elements = version_string.split('.', 4).map(&:strip).map{ |s| Integer(s) }
				major = elements[0] if major.nil?
				minor = elements[1] if minor.nil?
				patch = elements[2] if patch.nil?
				build = elements[3] if build.nil?
			end

			if tag_string.blank?
				tag_build = 300
			else
				elements = tag_string.split('.', 2).map(&:strip)
				tag = elements[0] if tag.nil?
				tag_version = elements[1] if tag_version.nil?
				unless tag_version.nil?
					tag_version = tag_version.to_i
					tag_build = tag_version
					case tag
						when nil
							tag_build += 300
							tag = nil
						when "alpha"
							# do nothing
						when "beta"
							tag_build += 100
						when "rc"
							tag_build += 200
						when "final"
							tag_build += 300
							tag = nil
						else
							STDERR.puts "Unknown Tag: #{tag}"
							exit 2
					end
				end
			end
		end
		@major = major
		@minor = minor
		@patch = patch
		@build = build || tag_build
		@tag = tag
		@tag_version = tag_version || build
	end

	def to_s
		string = "#{@major || 0}.#{@minor || 0}.#{@patch || 0}.#{@build || 0}"
		return append_tag(string)
	end
	alias_method :to_str, :to_s

	def to_manifest
		string = "#{@major || 0}.#{@minor || 0}.#{@patch || 0}"
		return append_tag(string)
	end

	def to_assembly
		return "#{@major || 0}.#{@minor || 0}.#{@patch || 0}.*"
	end

	def to_file
		return "#{@major || 0}.#{@minor || 0}.#{@patch || 0}.#{@build || 0}"
	end
end

class Module
	def add_accessors(recurse: false)
		self.instance_variables.each { |var|
			self.class.class_eval {
				attr_accessor var[1..-1]
			}
		}
		if (recurse)
			self.children.each { |child|
				child.add_accessors(recurse: recurse)
			}
		end
	end

	def all_nil?(recurse: false)
		self.instance_variables.each { |var|
			value = instance_variable_get(var)
			return false unless value.nil?
		}

		if (recurse)
			self.children.each { |child|
				return false unless child.all_nil?(recurse: recurse)
			}
		end

		return true
	end

	def children = self.constants.collect { |c| self.const_get(c) }.select { |m| m.instance_of?(Module)}
end

module Options
	@quiet = false
	@dry_run = false
	@force = false
	@pretty_manifest = false

	module Version
		@string = nil
		@major = nil
		@minor = nil
		@patch = nil
		@build = nil
		@tag = nil
		@tag_version = nil
	end

	add_accessors(recurse: true)
end

def value_arg(name)
	return [/\A--#{name}(?:[:=])(.*)\z/]
end

def bool_arg(name)
	return [/\A--(|no-)#{name}\z/, /\A--#{name}([\+\-])\z/]
end

def parse_bool(full_arg, arg)
	return true if (full_arg[1] != '-')
	case arg
	when 'no-', '-'
		return false
	when '', '+'
		return true
	else
		raise Error.new("Unknown boolean flag: '#{arg}'")
	end
end

def parse_args(*args)
	in_args = true

	i = 0

	pop_arg = lambda { |arg|
		result = args[i]
		fail("'#{arg}' expects parameter") if result.nil?
		i += 1
		return result
	}

	while i < args.length
		arg = args[i]
		i += 1

		if in_args
			case arg
			when *bool_arg('quiet'), '-q'
				Options::quiet = parse_bool(arg, $1)
			when *bool_arg('force'), '-f'
				Options::force = parse_bool(arg, $1)
			when *bool_arg('dry'), *bool_arg('dry-run')
				Options::dry_run = parse_bool(arg, $1)
			when *bool_arg('pretty'), *bool_arg('pretty-manifest')
				Options::pretty_manifest = parse_bool(arg, $1)
			when *value_arg('major')
				Options::Version::major = $1.blank || pop_arg[]
			when *value_arg('minor')
				Options::Version::minor = $1.blank || pop_arg[]
			when *value_arg('patch')
				Options::Version::patch = $1.blank || pop_arg[]
			when *value_arg('build')
				Options::Version::build = $1.blank || pop_arg[]
			when *value_arg('tag')
				Options::Version::tag = $1.blank || pop_arg[]
			when *value_arg('tag-version')
				Options::Version::tag_version = $1.blank || pop_arg[]
			when '--'
				in_args = false
			when /\A--(.*)\z/
				fail "Unknown Parameter: '#{$1}'"
			else
				Options::Version::string = arg
			end
		else
			Options::Version::string = arg
		end
	end
end

parse_args(*ARGV)

if Options::Version::all_nil?(recurse: true)
	version = nil
	begin
		version = Paths::Version.read().strip
		Options::Version::string = version
	rescue
		# nothing to do here
	end
	fail "No version input provided" if Options::Version::string.nil?
end

$version = SemanticVersion.new(
	string: Options::Version::string,
	major: Options::Version::major,
	minor: Options::Version::minor,
	patch: Options::Version::patch,
	build: Options::Version::build,
	tag: Options::Version::tag,
	tag_version: Options::Version::tag_version
)

unless Options::quiet
	puts "Version    : '#{$version}'"
	indent_puts "Manifest : '#{$version.to_manifest}'"
	indent_puts "Assembly : '#{$version.to_assembly}'"
	indent_puts "File     : '#{$version.to_file}'"
	puts
end

def update_props
	project = nil
	File.open(Paths::Props.to_s, "r:bom|utf-8") { |file|
		project = Nokogiri::XML(file.read)
	}
	changed = false
	projectNode = project.at_xpath('//Project')

	new_version = $version.to_manifest
	new_assembly = $version.to_assembly
	new_file = $version.to_file

	projectNode.children.each { |child|
		next unless child.name == "PropertyGroup"
		version_tag = child.at_xpath("Version")&.children&.at(0)
		assemblyversion_tag = child.at_xpath("AssemblyVersion")&.children&.at(0)
		fileversion_tag = child.at_xpath("FileVersion")&.children&.at(0)

		if !version_tag.nil? && (Options::force || version_tag.to_s != new_version)
			version_tag.content = new_version
			changed = true
		end

		if !assemblyversion_tag.nil? && (assemblyversion_tag.to_s != new_assembly)
			assemblyversion_tag.content = new_assembly
			changed = true
		end

		if !fileversion_tag.nil? && (fileversion_tag.to_s != new_file)
			fileversion_tag.content = new_file
			changed = true
		end
	}

	if !changed
		puts "Project Properties Unchanged" unless Options::quiet
		return
	end

	puts "New Props: ", project unless Options::quiet

	unless Options::dry_run
		File.open(Paths::Props.to_s, "w:bom|utf-8") { |file|
			file.write(project.to_s)
		}
	end
end

def update_manifest(project)
	manifest = nil
	File.open(project.manifest.to_s, "r:bom|utf-8") { |file|
		manifest = JSON.parse(file.read)
	}
	if !Options::force && manifest["Version"] == $version.to_manifest
		puts "Manifest Unchanged (#{manifest["Version"]})" unless Options::quiet
		return
	end
	manifest["Version"] = $version.to_manifest
	pretty = JSON.pretty_generate(manifest);
	puts "New Manifest: ", pretty unless Options::quiet
	unless Options::dry_run
		File.open(project.manifest.to_s, "w:bom|utf-8") { |file|
			file.write(Options::pretty_manifest ? pretty : JSON.dump(manifest))
		}
	end
end

def update_assembly(project)
	tags, status = Open3.capture2('git', '-C', __dir__, 'describe', '--tags')
	unless status.success?
		eputs "Could not extract git tags info"
	end
	cl, status = Open3.capture2('git', '-C', __dir__, 'rev-parse', '--short', 'HEAD')
	unless status.success?
		eputs "Could not extract git changelist"
	end
	hostname = Socket.gethostname&.strip || "unknown"

	assembly = nil
	File.open(project.assembly.to_s, "r:bom|utf-8") { |file|
		assembly = file.read
	}

	lines = assembly.split("\n")

	new_changelist = "#{cl.strip}:#{tags.strip}"
	update_attribute = lambda { |name, new|
		prefix = "[assembly: #{name}(\"";
		suffix = "\")]"

		line_idx = lines.find_index{ |l| l.strip.start_with?(prefix) }
		if line_idx == -1 || line_idx.nil?
			raise "Could not find #{name} attribute in assembly file"
		end
		current = lines[line_idx][prefix.length...-suffix.length]
		if current == new
			false
		else
			lines[line_idx] = "#{prefix}#{new}#{suffix}"
			true
		end
	}

	changed = false
	changed = update_attribute["ChangeList", new_changelist] || changed
	changed = update_attribute["BuildComputerName", hostname] || changed
	changed = update_attribute["FullVersion", $version.to_s] || changed

	unless changed
		puts "Assembly Unchanged" unless Options::quiet
	end

	puts "New Assembly: ", lines unless Options::quiet

	unless Options::dry_run
		File.open(project.assembly.to_s, "w:bom|utf-8") { |file|
			file.write(lines.join("\n"))
		}
	end
end

update_props 

Projects.each { |project|
	puts "Updating Project: #{project.to_s}" unless Options::quiet

	update_manifest(project)
	update_assembly(project)
}
