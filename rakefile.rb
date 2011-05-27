require 'rake'
require 'rake/clean'
require 'time'

# Ensure a path is in cygwin format (used for rsync)
def cyg_path(path)
	if /^rsync:/ =~ path # If it's an rsync remote path, don't prepend /cygdrive to the front of it
		path.gsub('\\','/')
	else
		"/cygdrive/#{File.expand_path(path).gsub('\\','/').gsub(':','')}"
	end
end

# Ensure a path is in ruby format (use / for directory seperators)
def ruby_path(path)
	path = File.expand_path path
	path.gsub('\\','/')
end

# Ensure a path is in windows format
def win_path(path)
	path = File.expand_path path
	path.gsub('/','\\')
end

CSPROJ = FileList["#{Rake.original_dir}/*.csproj"]

# Get from the WORKSPACE environment variable, if it exists, otherwise
# use ..\\workspace
WORKSPACE = win_path(ENV['WORKSPACE'] || File.expand_path("..\\workspace", Rake.original_dir))
ARTIFACTS = "#{WORKSPACE}\\artifacts"
TESTS = "#{WORKSPACE}\\tests"
TEMP = "#{WORKSPACE}\\temp"

LOGFILE = "#{WORKSPACE}\\rakelog.txt"

REPORTS = "#{WORKSPACE}\\reports"
TESTRESULTS = "#{REPORTS}\\TestsRan.txt"
COVERAGERESULTS = "#{REPORTS}\\coverage.xml"
COVERAGEREPORTS = "#{REPORTS}\\ncover"

CLEAN.include(TEMP, LOGFILE)
# Automatically includes everything in CLEAN as well
CLOBBER.include(ARTIFACTS,REPORTS,TESTS)


desc "Runs tests; puts report in #{REPORTS}"
task :test => TESTRESULTS
desc "Runs coverage; puts report in #{REPORTS}"
task :coverage => COVERAGEREPORTS

# Creates the WORKSPACE directory if it doesn't exist
file WORKSPACE do
	# Don't route this command through shell because shell records
	# stuff in the logfile, which is in the workspace. 
	# So we have to create the workspace before we can create the logfile
	`mkdir #{WORKSPACE}` if (!File.exists? WORKSPACE)
end

# Creates the logfile if it doesn't exist
file LOGFILE => [WORKSPACE, :clearLog] do
	shell "echo \"Rake log file\" >> #{LOGFILE}"
end

task :clearLog => [WORKSPACE] do
	FileUtils.rm_rf LOGFILE if File.exists? LOGFILE
end

# Create the reports directory if it doesn't exist
file REPORTS => [LOGFILE, WORKSPACE] do
	shell "mkdir #{REPORTS}" if (!File.exists? REPORTS)
end

file TEMP => [LOGFILE, WORKSPACE] do
	shell "mkdir #{TEMP}" if (!File.exists? TEMP)
end

# Compiles the project, and sticks it in the ARTIFACTS directory
# Most of the other tasks work on artifacts, rather than the source code
desc "Creates artifacts in #{ARTIFACTS}"
task :build => [LOGFILE, TEMP, CSPROJ] do
	message "Building..."
	compile ARTIFACTS
end

# Builds the test assembly and puts it in TESTS
file TESTS => [LOGFILE, TEMP, CSPROJ] do
	message "Building tests..."
	compile TESTS
end

# Creates the TESTRESULTS file by running the tests
# Note: if there are no tests, no file will be created
file TESTRESULTS => [LOGFILE, REPORTS, TESTS] do
	message "Testing..."
	files = Dir.glob "#{ruby_path(TESTS)}/**/*.Tests.dll"
	message "  No tests found" if files.count == 0
	files.each do |file|
		message "  Running tests on #{file}"
		basename = File.basename file
		shell "nunit-console.exe /xml=#{REPORTS}\\Test#{basename}.xml #{win_path file}", true
	end
	shell "echo done > #{TESTRESULTS}"
end

# Creates the COVERAGERESULTS file, by running ncover
# on the test dlls in ARTIFACTS
# As with tests, there will no file if there are no test dlls
file COVERAGERESULTS => [LOGFILE, REPORTS, TESTS] do
	message "Generating test coverage..."
	files = Dir.glob "#{ruby_path(TESTS)}/**/*.Tests.dll"
	message "  No test found" if files.count == 0
	files.each do |file|
		message "  Running coverage on #{file}"
		basename = File.basename file
		shell "ncover.console.exe //x #{COVERAGERESULTS} nunit-console.exe #{win_path file}", true
	end
end

file COVERAGEREPORTS => [LOGFILE, COVERAGERESULTS] do
	message "Writing test coverage reports..."
	shell "ncover.reporting.exe \"#{COVERAGERESULTS}\" //or FullCoverageReport:Html //op \"#{COVERAGEREPORTS}\""
end


###############################################
# Helper functions
###############################################

# Run a shell command, an optionally send the results
# to the console
# Results will be logged to the logfile automatically
def shell(command, results_to_console = false)
	res = `#{command}`
	was_error = (Integer($?) != 0)
	# Write to the log file in a command prompt style
	logtext =  "    (#{Dir.getwd})\n    #{command}\n    #{res}"
	# Add error information if there was an error
	logtext = "#{logtext}\nError in shell command (exit code #{$?})" if was_error
	log logtext
	# Write the results if we were told to
	puts res if results_to_console and !was_error
	puts logtext if was_error
	raise "Error occurred in shell command" if was_error
end

# Write a message to the console, and log it
def message(text)
	puts text
	log text
end
	
# Write to the log file
def log(text)
	File.open(LOGFILE, 'a') do |f| 
		f.write text + "\n"
	end
end
		

# Use rsync to copy a directory to another
def sync(from, to)
	shell "rsync -rmpulzDI --chmod=ugo=rwX --delete --force #{cyg_path from}/* #{cyg_path to}/"
end

# Compiles from the current directory and puts the results in "to"
def compile(to)
	# Compile the C#
	message "  Compiling C#..."
	shell "msbuild /clp:Summary;Verbosity=minimal;ErrorsOnly /nologo /p:Configuration=Release;DebugSymbols=false /verbosity:minimal"

	# If it exists, just copy from bin/Release
	if File.exists? "bin/Release/"
		from = "bin/Release/"
	else
		message "  No bin/Release directory found, compiling as a website"
		siteurl = ENV['SITEURL'] || raise("SITEURL environment variable was not set")
		# Otherwise, it's a website, use the msbuild web deploy task to move it up, and asp compile it
		# This copies just the files that the website needs to the TEMP\\compiled directory (it leaves out .cs files, and anything
		# that isn't included in the project file
		message "  Copying web application..."
		shell "msbuild /clp:Summary;Verbosity=minimal;ErrorsOnly /target:_WPPCopyWebApplication /nologo /p:Configuration=Release;WebProjectOutputDir=#{TEMP}\\compiled;DebugSymbols=false /verbosity:minimal"
		# This pre-compiles the website, so all the .aspx files are converted into actual code
		# This catches syntax errors in our ASPX, and makes for faster websites, since ASP .NET usually does this 
		# compilation at run-time
		message "  Pre-compiling web application..."
		shell "aspnet_compiler -f -v \"#{siteurl}\" -p #{TEMP}\\compiled -fixednames -nologo #{TEMP}\\asp_compiled"
		from = "#{WORKSPACE}/temp/asp_compiled"
	end
		
	# Copy our build files from wherever they are to the ARTIFACTS directory
	message "  Copying to folder: #{to}"
	sync from, to
end


####################################
# Script part - run when rake starts up
####################################

# If there are any other rake-*.rb files between the location of this rakefile and
# where we originally called rake from, include them
# For example, if this file is in /aqueduct, and we call rake from /aqueduct/project/trunk,
# then include file like these: /aqueduct/rake-deploy.rb, /aqueduct/project/trunk/rake-foobar.rb
dir = Rake.original_dir
# Get the location of this rakefile
this_file_dir = File.dirname(__FILE__)
to_include = []
done = false
while !done
	# Load them in alphabetical order (keep in mind we're reversing them later)
	Dir.glob("#{File.expand_path dir}/rake-*.rb").sort.each {|f| to_include << f}
	done = dir == this_file_dir
	dir = File.expand_path "..", dir
end
# Reverse the order we load the other rakefiles in
# We do this so that we load the files higher in the directory tree first,
# so that if lower files rely on things defined in higher files, they'll still
# work
to_include.reverse.each do |f|
	puts "Including #{f}"
	require f
end
	
# Change back to the original dir rake was called from
Dir.chdir Rake.original_dir

