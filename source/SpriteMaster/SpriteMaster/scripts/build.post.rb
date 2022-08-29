#!ruby
require 'pathname'
require 'fileutils'
require 'open3'
require 'set'

EnableMerge = false
EnableStrip = false

begin
TAB = '  '

def fail(*msg)
	puts *msg
	exit -1
end

ConstTypes = {
	:SolutionDir => Pathname,
	:OutDir => Pathname,
	:Mono => Pathname,
	:GamePath => Pathname,
	:TempAssembly => String,
	:DotNetRuntime => Pathname,
	:ILRepack => Pathname,
	:ILStrip => Pathname,
}

def set_const(name, value)
	if [String, Pathname].any?(&value.method(:is_a?))
		# escape the final backslash if it exists
		value = value.to_s
		value += '\\' if value[-1] == '\\'
		# enquote
		value = "'#{value}'"
	end
	type = ConstTypes[name.to_sym]
	if type.nil?
		eval("#{name} = #{value}")
	else
		eval("#{name} = #{type}.new(#{value})")
	end
end
def default_const(name, value) = (set_const(name, value) unless Object.const_defined?(name))
File.instance_eval {
	def to_win(str) = str.to_s.gsub('/', '\\')
	def to_unix(str) = str.to_s.gsub('\\', '/')
}
Pathname.prepend Module.new {
	def to_win = File.to_win(self)
	def to_unix = File.to_unix(self)
	def initialize(*args) = super(File.join(*args.map(&File.method(:to_unix))))
}
Pathname.instance_eval {
	def join(*args) = Pathname.new(*args)
}
String.prepend Module.new {
	def eat(count = 1) = self[count..-1]
	def eat!(count = 1) = self.replace(self.eat(count))
	def tab(depth = 1, with = TAB) = "#{with * depth}#{self}"
	def tab!(depth = 1, with = TAB) = self.insert(0, with * depth)
	def tabs?(with = tab) = self[/\A#{with}*/].size
	def enquote(with = '\'') = "#{with}#{self}#{with}"
	def enquote!(with = '\'') = self.insert(0, with).insert(self.length, with)
	def enquoted?(with = '\'') = (self.length >= with.length * 2) && self.start_with?(with) && self.end_with?(with)
}

ARGV.each { |arg|
	case arg
	when /^(.+)[=](.*)$/
		puts "<#{$2}>"
		set_const($1, $2)
	when /^(-|--|\/|)32$/
		set_const(:Copy32, true)
	when /^(-|--|\/|)64$/
		set_const(:Copy64, true)
	else
		raise "Unknown Parameter #{arg}"
	end
}

defined? :SolutionDir
defined? :OutDir
default_const(:Copy32, false)
default_const(:Copy64, !Copy32)
default_const(:Mono, Pathname.new("C:", "Program Files", "Mono", "bin"))
default_const(:GamePath, File.join("C:", "Program Files (x86)", "Steam", "steamapps", "common", "Stardew Valley"))
default_const(:Primary, "SpriteMaster.dll")
default_const(:TempAssembly, "SpriteMaster.merged.dll")
default_const(:DotNetRuntime, File.join("C:", "Program Files", "dotnet", "packs", "Microsoft.NETCore.App.Ref", "5.0.0", "ref", "net5.0"))
default_const(:IgnoreModFilePatterns, [])

set_const(:Copy32, false)
set_const(:Copy64, false)

IsDebug = OutDir.to_s.include?("Debug")

FileIgnorePatterns = IgnoreModFilePatterns.split(',').map(&:strip).map(&Regexp.method(:new))

puts "SolutionDir: #{SolutionDir}"
puts "OutDir: #{OutDir}"
puts "FileIgnorePatterns: #{FileIgnorePatterns}"
puts "GamePath: #{GamePath}"
puts "Mono: #{Mono}"
puts "ILRepack: #{ILRepack}"
puts "ILStrip: #{ILStrip}"
puts "Primary: #{Primary}"
puts "32-bit: #{Copy32 ? "enabled" : "disabled"}"
puts "64-bit: #{Copy64 ? "enabled" : "disabled"}"

PrebuiltPaths = [SolutionDir + 'Libraries']

class Library
	attr_reader :path, :name, :ext
	def self.is?(path) = ['.dll', '.so', '.dylib'].include?(File.extname(path).downcase)
	def self.dotnet?(path)
		str, stat = Open3.capture2e('file', '-b0', path.to_s)
		return stat.success? && str.include?("assembly")
	end
	def self.excluded?(path)
		return true if path.to_s.include?('x86') && !Copy32
		return true if path.to_s.include?('32') && !Copy32
		return true if path.to_s.include?('64') && !Copy64
		return false
	end
	def initialize(path)
		@path = Pathname.new(path)
		@name = @path.basename.to_s
		@ext = @path.extname.downcase
		@dotnet = Library.dotnet?(path)
		@primary = @dotnet ? (@name == Primary) : false
	end
	def dotnet? = @dotnet
	def primary? = @primary
	def to_s = @path.to_s
end

puts "Copying licenses..."
LicensesSource = SolutionDir + "Licenses"
LicensesDest = OutDir + "Licenses"
FileUtils.rm_rf(LicensesDest)
FileUtils.cp_r(LicensesSource, LicensesDest, preserve: true)

puts "Copying prebuilt libraries..."
PrebuiltPaths.each { |directory|
	if directory.directory?
		directory.glob("**/*") { |f|
			next if Library.excluded?(f)
			dest = OutDir + f.relative_path_from(directory)
			puts "'#{f}' -> '#{dest}'"
			FileUtils.cp(f, dest, preserve: true)
		}
	end
}

$libraries = []

puts "Assembling library list..."
OutDir.glob("**/*") { |f|
	next unless Library.is?(f)
	next if Library.excluded?(f)
	$libraries << Library.new(f)
}
$libraries.freeze

puts "Stripping prebuilt libraries..."
$libraries.each { |lib|
	next if lib.dotnet?

	puts lib.to_s.tab
	case lib.ext
		when '.dylib'
			system('llvm-strip', '--discard-all', '--strip-debug', lib.path.to_s) rescue nil
		when '.dll', '.so'
			system('strip', '--discard-all', '--strip-unneeded', lib.path.to_s) rescue nil
			system('llvm-strip', '--discard-all', '--strip-unneeded', lib.path.to_s) rescue nil
	end
}

unless Copy32
	puts "Deleting 32-bit libraries..."
	FileUtils.rm_rf(OutDir + 'x86')
end

unless Copy64
	puts "Deleting 64-bit libraries..."
	FileUtils.rm_rf(OutDir + 'x64')
end

def all_assemblies(dir)
	result = []
	dir.glob("*") { |file|
		next if ['.dll', '.exe'].none?{ |ext| ext == file.extname.downcase }
		result << file if Library.dotnet?(file)
	}
	return result
end

$unfiltered_dotnet_libraries = $libraries.select(&:dotnet?).select{ |l| l.name != TempAssembly }
$unfiltered_dotnet_libraries.freeze
$filtered_dotnet_libraries = FileIgnorePatterns.empty? ?
	$unfiltered_dotnet_libraries :
	$unfiltered_dotnet_libraries.select { |lib| FileIgnorePatterns.none?{ |p| p =~ lib.name.to_s } }
$filtered_dotnet_libraries.freeze

def loud_call(*args)
	args = args.map(&:to_s)
	puts "< #{args.map{|s| s.match(/\s/) ? s.enquote('"') : s}.join(' ')} >"
	STDOUT.flush
	system(*args)
	return $?
end

def remove_common_prefix(strings)
	strings = strings.map{|s| s.to_s.split('/')}
	first = strings[0]
	
	while strings.all?{ |s| s[0] == first[0] } && !first.empty?
		strings.each(&:shift)
	end
	
	return strings.map{|s| Pathname.new(*s) }
end

def RepackBinary(libraries:, target:)
	il_repack_binary = (ILRepack + "ILRepack.exe") rescue nil
	return false if il_repack_binary.nil? || !il_repack_binary.executable?
	
	puts "Linking .NET assemblies..."

	short_list = remove_common_prefix(libraries.map(&:path))
	puts "Merging libraries: ", short_list.map(&:to_s).map(&:tab), ""
	
	primary = libraries.find(&:primary?)
	others = libraries.reject(&:primary?)
	puts "primary: #{primary}"
	#puts "others: #{others}"

	STDOUT.flush

	return loud_call(
		il_repack_binary,
		#"/union",
		"/internalize",
		"/noRepackRes",
		"/parallel",
		"/out:#{target}",
		"/lib:#{GamePath}",
		"/lib:#{GamePath + 'smapi-internal'}",
		"/lib:#{GamePath + 'Mods' + 'ConsoleCommands'}",
		"/lib:#{GamePath + 'Mods' + 'ErrorHandler'}",
		"/lib:#{DotNetRuntime}",
		primary.path,
		*others.map(&:path)
	).success?
end

if EnableMerge && !IsDebug
	fail "Failed to repack binary" unless RepackBinary(
		libraries: $filtered_dotnet_libraries,
		target: OutDir + TempAssembly
	)

	# delete all .NET assemblies
	puts "Deleting merged binaries..."
	$unfiltered_dotnet_libraries.map(&:path).each(&:delete)
	$unfiltered_dotnet_libraries.map{|l| l.path.sub_ext(".pdb")}.each { |f|
		f.delete rescue nil
	}
	FileUtils.mv(
		OutDir + TempAssembly,
		OutDir + Primary
	)
	FileUtils.mv(
		OutDir + Pathname.new(TempAssembly).sub_ext(".pdb"),
		OutDir + Pathname.new(Primary).sub_ext(".pdb")
	)
end

def StripBinary(library:, target:)
	il_strip_binary = (ILStrip + "tools" + "BrokenEvent.ILStrip.CLI.exe") rescue nil
	return false if il_strip_binary.nil? || !il_strip_binary.executable?
	
	puts "Stripping merged library..."
	
	assemblies = Set.new
	[
		GamePath,
		GamePath + 'smapi-internal',
		GamePath + 'Mods' + 'ConsoleCommands',
		GamePath + 'Mods' + 'ErrorHandler',
	].each { |dir|
		assemblies.merge(all_assemblies(dir))
	}
	
	temp_dir = Pathname.new('.temp_assemblies')
	FileUtile.rm_rf(temp_dir) rescue nil
	FileUtils.mkdir_p(temp_dir)
	begin
		sub_assemblies = Set.new
		assemblies.each { |assembly|
			dir = temp_dir + assembly.dirname.basename
			subname = dir + assembly.basename
			FileUtils.mkdir_p(dir)
			FileUtils.ln_sf(assembly, subname)
			#puts "linking #{assembly} -> #{subname}"
			sub_assemblies << subname
		}
	
		result = loud_call(
			il_strip_binary,
			library,
			target,
			'-e', 'SpriteMaster.SpriteMaster',
			'-e', 'SpriteMaster.Harmonize.Patches.Cleanup',
			'-e', 'SpriteMaster.Harmonize.Patches.NVTT',
			'-e', 'SpriteMaster.Harmonize.Patches.PGraphicsDevice',
			'-e', 'SpriteMaster.Harmonize.Patches.PGraphicsDeviceManager',
			'-e', 'SpriteMaster.Harmonize.Patches.PSpriteBatch.Begin',
			'-e', 'SpriteMaster.Harmonize.Patches.PSpriteBatch.PlatformRenderBatch',
			'-e', 'SpriteMaster.Harmonize.Patches.PSpriteBatch.Patch.Draw',
			'-e', 'SpriteMaster.Harmonize.Patches.PTexture2D',
			*sub_assemblies.flat_map { |e| ['-import', e] },
			'-import', DotNetRuntime + 'System.Runtime.dll'
		)
	ensure
		FileUtils.rm_rf(temp_dir)
	end
end

if EnableStrip && !IsDebug
	fail "Failed to strip binary" unless StripBinary(
		library: $filtered_dotnet_libraries.find(&:primary?),
		target: OutDir + TempAssembly
	)
end

rescue => ex
	STDOUT.puts "Error: #{ex}"
	STDOUT.puts ex.backtrace.map(&:tab)
end
