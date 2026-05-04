const url = require('url');
const errorHandler = require('./errorAPI');
const { Pulpit } = require('../../index');

function addPulpit(request, response, body) {
    Pulpit.create({
        pulpit: body.pulpit,
        pulpit_name: body.pulpit_name,
        faculty: body.faculty
    }).then(result => {
        response.end(JSON.stringify(result));
    }).catch(error => errorHandler(response, 500, error.message));
}

function updatePulpit(request, response, body) {
    const updateData = {
        pulpit_name: body.pulpit_name
    };
    
    if (body.faculty !== undefined) {
        updateData.faculty = body.faculty;
    }
    
    Pulpit.update(updateData,
        { where: { pulpit: body.pulpit } })
        .then(result => {
            if (result[0] == 0) {
                throw new Error('Pulpit not exists');
            }
            else {
                const updatedData = {
                    pulpit: body.pulpit,
                    pulpit_name: body.pulpit_name,
                    faculty: body.faculty || null
                };
                response.end(JSON.stringify(updatedData));
            }
        }).catch(error => errorHandler(response, 500, error.message));
}

module.exports = function (request, response) {
    response.writeHead(200, { 'Content-Type': 'application/json; charset=utf-8' });

    switch (request.method) {

        //---------------------GET----------------------------
        case "GET":
            {
                Pulpit.findAll()
                    .then(result => {
                        response.end(JSON.stringify(result))
                    })
                    .catch(error => errorHandler(response, 500, error.message));

                break;
            }

        //---------------------POST----------------------------
        case "POST":
            {
                let body = '';
                if (url.parse(request.url).pathname === '/api/pulpits') {
                    request.on('data', function (data) {
                        body += data.toString();
                    });
                    request.on('end', function () {
                        body = JSON.parse(body);
                        addPulpit(request, response, body);
                    });

                    break;
                }
            }

        //---------------------PUT----------------------------
        case "PUT":
            {
                let body = '';
                if (url.parse(request.url).pathname === '/api/pulpits') {
                    request.on('data', function (data) {
                        body += data.toString();
                    });
                    request.on('end', function () {
                        body = JSON.parse(body);
                        updatePulpit(request, response, body);
                    });

                    break;
                }
            }

        //---------------------DELETE----------------------------
        case "DELETE":
            {
                const pulpitId = decodeURIComponent(request.url.split('/')[3]).trim();

                console.log('DELETE PULPIT ID:', pulpitId);

                // First check if pulpit exists
                Pulpit.findByPk(pulpitId)
                    .then(pulpit => {
                        if (!pulpit) {
                            errorHandler(response, 404, 'Pulpit not exists');
                            return;
                        }
                        
                        // Check for dependencies (example with Teacher and Subject models)
                        // You'll need to import these models at the top
                        const { Teacher, Subject } = require('../../index');
                        
                        return Promise.all([
                            Teacher.count({ where: { pulpit: pulpitId } }),
                            Subject.count({ where: { pulpit: pulpitId } })
                        ]).then(([teachersCount, subjectsCount]) => {
                            if (teachersCount > 0 || subjectsCount > 0) {
                                const dependencies = [];
                                if (teachersCount > 0) dependencies.push(`${teachersCount} teacher(s)`);
                                if (subjectsCount > 0) dependencies.push(`${subjectsCount} subject(s)`);
                                
                                errorHandler(response, 409, `Cannot delete pulpit: has dependencies - ${dependencies.join(', ')}`);
                                return null;
                            }
                            
                            // No dependencies, proceed with deletion
                            return Pulpit.destroy({ where: { pulpit: pulpitId } });
                        });
                    })
                    .then(resultD => {
                        if (resultD === null) return; // Already handled
                        
                        console.log('DELETE RESULT:', resultD);

                        if (resultD == 0) {
                            errorHandler(response, 404, 'Pulpit not exists');
                        }
                        else {
                            response.end(JSON.stringify({
                                message: 'Pulpit deleted successfully',
                                pulpit: pulpitId
                            }));
                        }
                    })
                    .catch(error => {
                        if (error.name === 'SequelizeForeignKeyConstraintError') {
                            errorHandler(response, 409, 'Cannot delete pulpit: it has existing related records');
                        } else {
                            console.log('DELETE ERROR:', error.message);
                            errorHandler(response, 500, error.message);
                        }
                    });

                break;
            }

        default:
            {
                errorHandler(response, 405, 'Method Not Allowed');
                break;
            }
    }
}