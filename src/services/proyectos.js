// Importe de rutas para apis, funciones y utilidades
import { PROYECTOS_API, PROYECTOS_GET_BY_CODIGO_API, PROYECTOS_ULTIMO_CODIGO_API } from '../config/settings.js';
import { showSuccess, showError, showAlert, showConfirmation } from '../utils/sweetAlert.js';
import { getData, deleteData, postData, putData } from '../data/methods.js';

// Obtener todos los proyectos y mostrarlos en DataTable
const obtenerTodosLosProyectos = async () => {
    try {
        const response = await getData(PROYECTOS_API);

        if (response && response.data) {
            
            // Inicializar DataTable con los datos obtenidos
            $('#tablaProyectos').DataTable({
                destroy: true,
                data: response.data,
                columns: [
                    { data: "id" },
                    { data: "codigo" },
                    { data: "nombre" },
                    { data: "municipio" },
                    { data: "departamento" },
                    { 
                        data: "fechaInicio",
                        render: function(data) {
                            // Formatear fecha para mejor visualización
                            return new Date(data).toLocaleDateString('es-GT');
                        }
                    },
                    { 
                        data: "fechaFin",
                        render: function(data) {
                            // Formatear fecha para mejor visualización
                            return new Date(data).toLocaleDateString('es-GT');
                        }
                    },
                    {
                        data: null,
                        render: function (data, type, row) {
                            return `
                                <div class="btn-group" role="group">
                                    <button onclick="editarProyecto(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-warning btn-sm">Editar</button>
                                    <button onclick="eliminarProyecto(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-danger btn-sm">Eliminar</button>
                                </div>
                            `;
                        }
                    }
                ],
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'copy',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-copy"></i> Copiar'
                    },
                    {
                        extend: 'excel',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-file-excel"></i> Excel'
                    },
                    {
                        extend: 'pdf',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-file-pdf"></i> PDF'
                    },
                    {
                        extend: 'print',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-print"></i> Imprimir'
                    }
                ],
                language: {
                    url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
                },
                responsive: true,
                order: [[0, 'desc']] // Ordenar por ID descendente por defecto
            });
        } else {
            showAlert("No se encontraron proyectos.", "warning");
            // Inicializar DataTable vacío para que se muestren los botones de exportación
            $('#tablaProyectos').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'copy',
                        className: 'btn btn-secondary'
                    },
                    {
                        extend: 'excel',
                        className: 'btn btn-secondary'
                    },
                    {
                        extend: 'pdf',
                        className: 'btn btn-secondary'
                    },
                    {
                        extend: 'print',
                        className: 'btn btn-secondary'
                    }
                ],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
                }
            });
        }
    } catch (error) {
        showError(error.message, "danger");
        throw error;
    }
};

// Función para obtener el último código de proyecto
const obtenerUltimoCodigo = async () => {
    try {
        const response = await getData(PROYECTOS_ULTIMO_CODIGO_API);
        if (response && response.data) {
            return response.data;
        }
        return "P-0000"; // Código por defecto
    } catch (error) {
        console.error('Error al obtener último código:', error);
        return "P-0000"; // Código por defecto en caso de error
    }
};

// Función para generar el siguiente código de proyecto
const generarSiguienteCodigo = (ultimoCodigo) => {
    if (!ultimoCodigo || !ultimoCodigo.startsWith('P-')) {
        return "P-0001";
    }
    
    const numeroStr = ultimoCodigo.substring(2);
    const numero = parseInt(numeroStr) || 0;
    const siguienteNumero = numero + 1;
    
    return `P-${siguienteNumero.toString().padStart(4, '0')}`;
};

// Función global para editar proyecto
window.editarProyecto = async (proyecto) => {
    try {
        // Obtener datos completos del proyecto por su código
        const response = await getData(PROYECTOS_GET_BY_CODIGO_API(proyecto.codigo));
        
        if (response && response.data) {
            const proyectoCompleto = response.data;
            
            // Actualizar el título del modal
            document.getElementById('proyectoModalLabel').textContent = 'Editar Proyecto';
            
            // Llenar los campos con los datos del proyecto
            document.getElementById('proyectoId').value = proyectoCompleto.id || '';
            document.getElementById('codigoProyecto').value = proyectoCompleto.codigo || '';
            document.getElementById('nombreProyecto').value = proyectoCompleto.nombre || '';
            document.getElementById('municipioProyecto').value = proyectoCompleto.municipio || '';
            document.getElementById('departamentoProyecto').value = proyectoCompleto.departamento || '';
            
            // Formatear fechas para el input type="date" (YYYY-MM-DD)
            if (proyectoCompleto.fechaInicio) {
                const fechaInicio = new Date(proyectoCompleto.fechaInicio);
                document.getElementById('fechaInicio').value = fechaInicio.toISOString().split('T')[0];
            }
            
            if (proyectoCompleto.fechaFin) {
                const fechaFin = new Date(proyectoCompleto.fechaFin);
                document.getElementById('fechaFin').value = fechaFin.toISOString().split('T')[0];
            }
            
            // Abrir el modal
            const modal = new bootstrap.Modal(document.getElementById('modalProyecto'));
            modal.show();
        } else {
            showError('No se pudieron obtener los datos completos del proyecto');
        }
    } catch (error) {
        showError('Error al cargar datos del proyecto: ' + error.message);
    }
};

// Función global para eliminar proyecto (versión con .then())
window.eliminarProyecto = (proyecto) => {
    showConfirmation(
        `¿Estás seguro de que deseas eliminar el proyecto "${proyecto.nombre}" (${proyecto.codigo})?`,
        "Sí, eliminar",
        "Cancelar"
    ).then(async (confirmado) => {
        if (confirmado) {
            try {
                const response = await deleteData(PROYECTOS_GET_BY_CODIGO_API(proyecto.codigo));
                
                if (response && response.success) {
                    showSuccess('Proyecto eliminado correctamente');
                    await obtenerTodosLosProyectos(); // Recargar la tabla
                } else {
                    showError(response?.message || 'Error al eliminar el proyecto');
                }
            } catch (error) {
                showError('Error al eliminar el proyecto: ' + error.message);
            }
        }
    });
};


// Función para guardar proyecto (crear o editar)
window.guardarProyecto = async () => {
    const form = document.getElementById('formProyecto');
    
    // Validar el formulario
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }
    
    try {
        const proyectoId = document.getElementById('proyectoId').value;
        const codigoProyecto = document.getElementById('codigoProyecto').value;
        const isEdit = proyectoId > 0;
        
        const proyectoData = {
            id: proyectoId || 0,
            codigo: codigoProyecto,
            nombre: document.getElementById('nombreProyecto').value,
            municipio: document.getElementById('municipioProyecto').value,
            departamento: document.getElementById('departamentoProyecto').value,
            fechaInicio: document.getElementById('fechaInicio').value,
            fechaFin: document.getElementById('fechaFin').value
        };
        
        let response;
        if (isEdit) {
            // Editar proyecto existente
            response = await putData(PROYECTOS_GET_BY_CODIGO_API(codigoProyecto), proyectoData);
        } else {
            // Crear nuevo proyecto
            response = await postData(PROYECTOS_API, proyectoData);
        }
        
        if (response && response.success) {
            showSuccess(response.message || (isEdit ? 'Proyecto actualizado correctamente' : 'Proyecto creado correctamente'));
            
            // Cerrar el modal y recargar la tabla
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalProyecto'));
            modal.hide();
            await obtenerTodosLosProyectos();
        } else {
            showError(response?.message || 'Error al guardar el proyecto');
        }
    } catch (error) {
        showError('Error al guardar el proyecto: ' + error.message);
    }
};

// Configurar eventos cuando el DOM esté cargado
document.addEventListener('DOMContentLoaded', function() {
    // Evento para el botón de guardar proyecto
    document.getElementById('btnGuardarProyecto').addEventListener('click', window.guardarProyecto);
    
    // Evento cuando se abre el modal para nuevo proyecto
    document.getElementById('modalProyecto').addEventListener('show.bs.modal', async function (e) {
        // Si es un nuevo proyecto (no edición), generar código automático
        if (!document.getElementById('proyectoId').value) {
            document.getElementById('proyectoModalLabel').textContent = 'Nuevo Proyecto';
            document.getElementById('codigoProyecto').value = 'Generando...';
            
            try {
                const ultimoCodigo = await obtenerUltimoCodigo();
                const nuevoCodigo = generarSiguienteCodigo(ultimoCodigo);
                document.getElementById('codigoProyecto').value = nuevoCodigo;
            } catch (error) {
                document.getElementById('codigoProyecto').value = 'P-0001';
                console.error('Error al generar código:', error);
            }
        }
    });
    
    // Evento cuando se cierra el modal
    document.getElementById('modalProyecto').addEventListener('hidden.bs.modal', function () {
        document.getElementById('formProyecto').reset();
        document.getElementById('proyectoId').value = '';
    });
});

// Inicializar la página
window.onload = async () => {
    await obtenerTodosLosProyectos();
};