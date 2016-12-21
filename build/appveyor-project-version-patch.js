var jsonfile = require("jsonfile");
var semver = require("semver");

var files = ["../src/imagesharp/project.json", "../src/ImageSharp.Drawing.Text/project.json"];

var semversion = semver.valid(process.env.mssemver);
files.forEach(function (f) {
    //update all the files in turn
    jsonfile.readFile(f, function (err, project) {
        project.version = semversion;
        jsonfile.writeFile(f, project, { spaces: 2 }, function (err) {
            console.error(err);
        });
    })
})
