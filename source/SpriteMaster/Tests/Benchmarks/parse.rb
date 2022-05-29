#!/usr/bin/env ruby

require 'Pathname'

def eputs(*msg)
	STDERR.puts(*msg)
end

def fail(*msg, code: -1)
	eputs(*msg)
	exit code
end

fail "no input file provided" if ARGV[0].nil?

$infile = Pathname.new(ARGV[0])
$outfile = Pathname.new(ARGV[0] + '.csv')

fail "input file '#{$infile}' does not exist" unless $infile.file?
fail "output file '#{$outfile}' already eexists" unless $outfile.file?
