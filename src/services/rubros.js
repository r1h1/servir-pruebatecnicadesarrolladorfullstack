// Importe de rutas para apis, funciones y utilidades
import {
    RUBROS_API, RUBROS_GET_BY_CODIGO_API, RUBROS_INACTIVAR_PROYECTO_API, RUBROS_COMPLETOS_API, RUBROS_PROYECTO_API,
    PROYECTOS_API 
} from '../config/settings.js';
import { showSuccess, showError, showAlert, showConfirmation } from '../utils/sweetAlert.js';
import { getData, deleteData, postData, putData } from '../data/methods.js';

// Variable global para almacenar los proyectos
let proyectosDisponibles = [];

// Obtener todos los rubros y mostrar
const obtenerTodosLosRubros = async () => {
    try {
        const response = await getData(RUBROS_COMPLETOS_API);

        if (response && response.data) {
            $('#tablaRubros').DataTable({
                destroy: true,
                data: response.data,
                columns: [
                    { data: "id" },
                    { data: "codigo" },
                    { data: "nombre" },
                    { data: "codigoProyecto" },
                    { data: "nombreProyecto" },
                    { data: "municipio" },
                    { data: "departamento" },
                    {
                        data: "fechaInicio",
                        render: function (data) {
                            return new Date(data).toLocaleDateString('es-GT');
                        }
                    },
                    {
                        data: "fechaFin",
                        render: function (data) {
                            return new Date(data).toLocaleDateString('es-GT');
                        }
                    },
                    {
                        data: null,
                        render: function (data, type, row) {
                            return `
                                <div class="btn-group" role="group">
                                    <button onclick="editarRubro(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-warning btn-sm">Editar</button>
                                    <button onclick="eliminarRubro(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-danger btn-sm">Eliminar</button>
                                </div>
                            `;
                        }
                    }
                ],
                dom: 'Bfrtip',
                buttons: [
                    { extend: 'copy', className: 'btn btn-secondary', text: '<i class="fas fa-copy"></i> Copiar' },
                    { extend: 'excel', className: 'btn btn-secondary', text: '<i class="fas fa-file-excel"></i> Excel' },
                    { extend: 'pdf', className: 'btn btn-secondary', text: '<i class="fas fa-file-pdf"></i> PDF' },
                    { extend: 'print', className: 'btn btn-secondary', text: '<i class="fas fa-print"></i> Imprimir' }
                ],
                language: { url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json' },
                responsive: true,
                order: [[0, 'desc']]
            });
        } else {
            showAlert("No se encontraron rubros.", "warning");
            $('#tablaRubros').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    { extend: 'copy', className: 'btn btn-secondary' },
                    { extend: 'excel', className: 'btn btn-secondary' },
                    { extend: 'pdf', className: 'btn btn-secondary' },
                    { extend: 'print', className: 'btn btn-secondary' }
                ],
                language: { url: 'https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json' }
            });
        }
    } catch (error) {
        showError(error.message, "danger");
        throw error;
    }
};

// Función para cargar proyectos en el select
const cargarProyectosEnSelect = async () => {
    try {
        const response = await getData(PROYECTOS_API);
        
        if (response && response.data) {
            proyectosDisponibles = response.data;
            const select = document.getElementById('proyectoSelect');
            
            // Limpiar opciones excepto la primera
            while (select.options.length > 1) {
                select.remove(1);
            }
            
            // Agregar proyectos al select
            proyectosDisponibles.forEach(proyecto => {
                const option = document.createElement('option');
                option.value = proyecto.id;
                option.textContent = `${proyecto.codigo} - ${proyecto.nombre}`;
                option.setAttribute('data-codigo', proyecto.codigo);
                option.setAttribute('data-nombre', proyecto.nombre);
                option.setAttribute('data-municipio', proyecto.municipio);
                option.setAttribute('data-departamento', proyecto.departamento);
                option.setAttribute('data-fechaInicio', proyecto.fechaInicio);
                option.setAttribute('data-fechaFin', proyecto.fechaFin);
                select.appendChild(option);
            });
        }
    } catch (error) {
        console.error('Error al cargar proyectos:', error);
        showError('No se pudieron cargar los proyectos');
    }
};

// Función para autocompletar campos cuando se selecciona un proyecto
const autocompletarCamposProyecto = (proyectoId) => {
    const proyecto = proyectosDisponibles.find(p => p.id == proyectoId);
    
    if (proyecto) {
        document.getElementById('idProyecto').value = proyecto.id;
        document.getElementById('codigoProyecto').value = proyecto.codigo || '';
        document.getElementById('nombreProyecto').value = proyecto.nombre || '';
        document.getElementById('municipio').value = proyecto.municipio || '';
        document.getElementById('departamento').value = proyecto.departamento || '';
        
        if (proyecto.fechaInicio) {
            const fechaInicio = new Date(proyecto.fechaInicio);
            document.getElementById('fechaInicio').value = fechaInicio.toISOString().split('T')[0];
        }
        
        if (proyecto.fechaFin) {
            const fechaFin = new Date(proyecto.fechaFin);
            document.getElementById('fechaFin').value = fechaFin.toISOString().split('T')[0];
        }
    } else {
        // Limpiar campos si no se encuentra el proyecto
        document.getElementById('idProyecto').value = '';
        document.getElementById('codigoProyecto').value = '';
        document.getElementById('nombreProyecto').value = '';
        document.getElementById('municipio').value = '';
        document.getElementById('departamento').value = '';
        document.getElementById('fechaInicio').value = '';
        document.getElementById('fechaFin').value = '';
    }
};

// Función para obtener último código de rubro
const obtenerUltimoCodigoRubro = async () => {
    try {
        // Lógica para obtener último código si existe endpoint
        return "R-0000";
    } catch (error) {
        console.error('Error al obtener último código:', error);
        return "R-0000";
    }
};

// Función para generar siguiente código de rubro
const generarSiguienteCodigoRubro = (ultimoCodigo) => {
    if (!ultimoCodigo || !ultimoCodigo.startsWith('R-')) {
        return "R-0001";
    }
    
    const numeroStr = ultimoCodigo.substring(2);
    const numero = parseInt(numeroStr) || 0;
    const siguienteNumero = numero + 1;
    
    return `R-${siguienteNumero.toString().padStart(4, '0')}`;
};

// Función global para editar rubro
window.editarRubro = async (rubro) => {
    try {
        const response = await getData(RUBROS_GET_BY_CODIGO_API(rubro.codigo));
        
        if (response && response.data) {
            const rubroCompleto = response.data;
            
            document.getElementById('rubroModalLabel').textContent = 'Editar Rubro';
            document.getElementById('rubroId').value = rubroCompleto.id || '';
            document.getElementById('codigo').value = rubroCompleto.codigo || ''; // Cambiado a 'codigo'
            document.getElementById('nombre').value = rubroCompleto.nombre || ''; // Cambiado a 'nombre'
            document.getElementById('active').checked = rubroCompleto.active === 1 || rubroCompleto.active === true;
            
            // Cargar proyectos y seleccionar el correspondiente
            await cargarProyectosEnSelect();
            document.getElementById('proyectoSelect').value = rubroCompleto.idProyecto || '';
            autocompletarCamposProyecto(rubroCompleto.idProyecto);
            
            const modal = new bootstrap.Modal(document.getElementById('modalRubro'));
            modal.show();
        } else {
            showError('No se pudieron obtener los datos del rubro');
        }
    } catch (error) {
        showError('Error al cargar datos del rubro: ' + error.message);
    }
};

// Función global para eliminar rubro
window.eliminarRubro = (rubro) => {
    showConfirmation(
        `¿Estás seguro de eliminar el rubro "${rubro.nombre}" (${rubro.codigo})?`,
        "Sí, eliminar",
        "Cancelar"
    ).then(async (confirmado) => {
        if (confirmado) {
            try {
                const response = await deleteData(RUBROS_GET_BY_CODIGO_API(rubro.codigo));
                
                if (response && response.success) {
                    showSuccess('Rubro eliminado correctamente');
                    await obtenerTodosLosRubros();
                } else {
                    showError(response?.message || 'Error al eliminar el rubro');
                }
            } catch (error) {
                showError('Error al eliminar el rubro: ' + error.message);
            }
        }
    });
};

// Función para guardar rubro
window.guardarRubro = async () => {
    const form = document.getElementById('formRubro');
    
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }
    
    try {
        const rubroId = document.getElementById('rubroId').value;
        const codigoRubro = document.getElementById('codigo').value; // Cambiado a 'codigo'
        const idProyecto = document.getElementById('idProyecto').value;
        const isEdit = rubroId > 0;
        
        if (!idProyecto) {
            showError('Debe seleccionar un proyecto');
            return;
        }
        
        const rubroData = {
            id: rubroId || 0,
            codigo: codigoRubro,
            nombre: document.getElementById('nombre').value, // Cambiado a 'nombre'
            idProyecto: parseInt(idProyecto),
            active: document.getElementById('active').checked ? 1 : 0
        };
        
        let response;
        if (isEdit) {
            response = await putData(RUBROS_GET_BY_CODIGO_API(codigoRubro), rubroData);
        } else {
            response = await postData(RUBROS_API, rubroData);
        }
        
        if (response && response.success) {
            showSuccess(response.message || (isEdit ? 'Rubro actualizado' : 'Rubro creado'));
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalRubro'));
            modal.hide();
            await obtenerTodosLosRubros();
        } else {
            showError(response?.message || 'Error al guardar el rubro');
        }
    } catch (error) {
        showError('Error al guardar el rubro: ' + error.message);
    }
};

// Configurar eventos
document.addEventListener('DOMContentLoaded', function() {
    // Evento para el botón de guardar
    document.getElementById('btnGuardarRubro').addEventListener('click', window.guardarRubro);
    
    // Evento cuando se selecciona un proyecto
    document.getElementById('proyectoSelect').addEventListener('change', function() {
        autocompletarCamposProyecto(this.value);
    });
    
    // Evento cuando se abre el modal
    document.getElementById('modalRubro').addEventListener('show.bs.modal', async function () {
        if (!document.getElementById('rubroId').value) {
            document.getElementById('rubroModalLabel').textContent = 'Nuevo Rubro';
            document.getElementById('codigo').value = 'Generando...'; // Cambiado a 'codigo'
            
            try {
                // Cargar proyectos en el select
                await cargarProyectosEnSelect();
                
                // Generar código de rubro
                const ultimoCodigo = await obtenerUltimoCodigoRubro();
                const nuevoCodigo = generarSiguienteCodigoRubro(ultimoCodigo);
                document.getElementById('codigo').value = nuevoCodigo; // Cambiado a 'codigo'
                
                // Limpiar campos de proyecto
                document.getElementById('proyectoSelect').value = '';
                autocompletarCamposProyecto('');
            } catch (error) {
                document.getElementById('codigo').value = 'R-0001'; // Cambiado a 'codigo'
            }
        }
    });
    
    // Evento cuando se cierra el modal
    document.getElementById('modalRubro').addEventListener('hidden.bs.modal', function () {
        document.getElementById('formRubro').reset();
        document.getElementById('rubroId').value = '';
        document.getElementById('proyectoSelect').value = '';
        autocompletarCamposProyecto('');
    });
});

// Inicializar
window.onload = async () => {
    await obtenerTodosLosRubros();
};