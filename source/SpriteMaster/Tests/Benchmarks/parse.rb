#!/usr/bin/env ruby

require 'Pathname'
require 'Set'
require 'csv'

def eputs(*msg)
	STDERR.puts(*msg)
end

def fail(*msg, code: -1)
	eputs(*msg)
	exit code
end

class String
	def is_i?
		 !!(self =~ /\A[-+]?[0-9]+\z/)
	end

	def is_f?
		is_i? || !!(self =~ /\A[-+]?[0-9]+\.[0-9]+\z/)
 end
end


fail "no input file provided" if ARGV[0].nil?

$infile = Pathname.new(ARGV[0])
$outfile = Pathname.new(ARGV[1] || ARGV[0].basename + 'results..csv')

$force = ARGV[2..-1].include?('-f') || ARGV[2..-1].include?('--force')
$rooted = ARGV[2..-1].include?('-r') || ARGV[2..-1].include?('--root')

if $rooted && !$outfile.absolute?
	$outfile = $infile.dirname + $outfile
end

fail "input file '#{$infile}' does not exist" unless $infile.file?

if $outfile.file?
	$outfile.delete if $force
	fail "output file '#{$outfile}' already exists" if $outfile.file?
end

csv = CSV.read($infile)

name_row = csv[0]

$indices = {
	:method => name_row.index("Method"),
	:dataSet => name_row.index("dataSet"),
	:mean => name_row.index("Mean"),
	:error => name_row.index("Error"),
	:standard_deviation => name_row.index("StdDev"),
}

results = csv[1..-1]

def nanoseconds(value)
	value.strip!
	value.gsub!(',', '')

	return nil if value == "NA"

	return value.to_f if value.is_f?

	value, suffix = value.split(' ', 2).map(&:strip)
	value = value.to_f

	case suffix.downcase
	when 'ps'
		value *= 0.001
	when 'ns'
		value = value
	when 'us', 'μs'
		value *= 1_000
	when 'ms'
		value *= 1_000_000
	when 's'
		value *= 1_000_000_000
	when 'm', 'min', 'mins', 'minute', 'minutes'
		value *= 60_000_000_000
	end
	return value
end

class Result
	attr_reader :mean
	attr_reader :error
	attr_reader :standard_deviation

	def initialize(row)
		@mean = nanoseconds(row[$indices[:mean]])
		@error = nanoseconds(row[$indices[:error]])
		@standard_deviation = nanoseconds(row[$indices[:standard_deviation]])

		self.freeze
	end

	def to_f; @mean; end
	alias_method :to_float, :to_f
end

class ResultMethod
	attr_reader :name
	attr_reader :results

	def initialize(name)
		@name = name.freeze
		@results = Hash.new
	end

	def append(dataset, result)
		@results[dataset] = result
	end
end

$methods = Hash.new

unique_datasets = Set.new

results.each { |result|
	dataset = result[$indices[:dataSet]]
	if dataset[0] == '(' && dataset.include?(')')
		dataset = dataset.split(')', 2)[1].strip
	end
	unique_datasets.add(dataset)

	method = result[$indices[:method]]
	method = method[1...-1] if (method.length > 1 && method.start_with?('\'') && method.end_with?('\''))
	method = method[1...-1] if (method.length > 1 && method.start_with?('\"') && method.end_with?('\"'))

	$methods[method] = ResultMethod.new(method) if $methods[method].nil?
	$methods[method].append(dataset, Result.new(result))
}

$datasets = unique_datasets.to_a

$sort = $datasets.all?(&:is_i?)

if $sort
	$datasets.sort_by!{ |d| Integer(d) }
end

CSV.open($outfile, "wb") { |out|
	max_means = Hash.new

		out << ["Mean", *$datasets]

		$methods.each { |name, method|
			results = method.results
			results = results.map { |k,v| [k, v] }

			if $sort
				results.sort_by!{ |v| Integer(v[0]) }
			end

			results.each { |v|
				current_max = max_means[v[0]]
				next if v[1].mean.nil?
				if current_max.nil?
					max_means[v[0]] = v[1].mean
				else
					max_means[v[0]] = [current_max, v[1].mean].max
				end
			}

			out << [name, *results.map{ |v| v[1].mean }]
		}

	out << []

		out << ["Relative Mean", *$datasets]

		$methods.each { |name, method|
			results = method.results
			results = results.map { |k,v| [k, v] }

			if $sort
				results = results.sort_by!{ |v| Integer(v[0]) }
			end

			out << [name, *results.map{ |v| v[1].mean.nil? ? nil : v[1].mean / max_means[v[0]] }]
		}
}
