// Importe de rutas para apis, funciones y utilidades
import {
    DONACIONES_API,
    DONACIONES_LISTAR_API,
    DONACIONES_LISTAR_RUBRO_API,
    DONACIONES_GET_BY_ID_API,
    DONACIONES_TOTAL_RUBRO_API,
    DONACIONES_REPORTE_PROYECTO_API,
    DONACIONES_TOP_DONANTES_API,
    RUBROS_COMPLETOS_API, // Para cargar rubros en el select
    PROYECTOS_API // Para cargar proyectos en el select
} from '../config/settings.js';
import { showSuccess, showError, showAlert, showConfirmation } from '../utils/sweetAlert.js';
import { getData, deleteData, postData, putData } from '../data/methods.js';

// Variables globales para almacenar rubros y proyectos
let rubrosDisponibles = [];
let proyectosDisponibles = [];

// Obtener todas las donaciones y mostrar
const obtenerTodasLasDonaciones = async () => {
    try {
        const response = await getData(DONACIONES_LISTAR_API);

        if (response && response.data) {
            $('#tablaDonaciones').DataTable({
                destroy: true,
                data: response.data,
                columns: [
                    { data: "id" },
                    { 
                        data: "idRubro",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "monto",
                        render: function(data) {
                            return data ? `Q ${parseFloat(data).toFixed(2)}` : 'Q 0.00';
                        }
                    },
                    { 
                        data: "fechaDonacion",
                        render: function(data) {
                            return data ? new Date(data).toLocaleDateString('es-GT') : 'N/A';
                        }
                    },
                    { 
                        data: "nombreDonante",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "codigoRubro",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "nombreRubro",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "codigoProyecto",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "nombreProyecto",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    {
                        data: "active",
                        render: function(data) {
                            return data === 1 ? 
                                '<span class="badge bg-success">Activo</span>' : 
                                '<span class="badge bg-danger">Inactivo</span>';
                        }
                    },
                    {
                        data: null,
                        render: function (data, type, row) {
                            return `
                                <div class="btn-group" role="group">
                                    <button onclick="editarDonacion(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-warning btn-sm">Editar</button>
                                    <button onclick="eliminarDonacion(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-danger btn-sm">Eliminar</button>
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
                order: [[0, 'desc']],
                pageLength: 10,
                lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]]
            });
        } else {
            showAlert("No se encontraron donaciones.", "warning");
            $('#tablaDonaciones').DataTable({
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

// Función para cargar rubros en el select
const cargarRubrosEnSelect = async () => {
    try {
        const response = await getData(RUBROS_COMPLETOS_API);
        
        if (response && response.data) {
            rubrosDisponibles = response.data;
            const select = document.getElementById('rubroSelect');
            
            // Limpiar opciones excepto la primera
            while (select.options.length > 1) {
                select.remove(1);
            }
            
            // Agregar rubros al select
            rubrosDisponibles.forEach(rubro => {
                const option = document.createElement('option');
                option.value = rubro.id;
                option.textContent = `${rubro.codigo} - ${rubro.nombre}`;
                option.setAttribute('data-codigo', rubro.codigo);
                option.setAttribute('data-nombre', rubro.nombre);
                option.setAttribute('data-codigoProyecto', rubro.codigoProyecto);
                option.setAttribute('data-nombreProyecto', rubro.nombreProyecto);
                select.appendChild(option);
            });
        }
    } catch (error) {
        console.error('Error al cargar rubros:', error);
        showError('No se pudieron cargar los rubros');
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
                select.appendChild(option);
            });
        }
    } catch (error) {
        console.error('Error al cargar proyectos:', error);
        showError('No se pudieron cargar los proyectos');
    }
};

// Función para autocompletar campos cuando se selecciona un rubro
const autocompletarCamposRubro = (rubroId) => {
    const rubro = rubrosDisponibles.find(r => r.id == rubroId);
    
    if (rubro) {
        document.getElementById('codigoRubro').value = rubro.codigo || '';
        document.getElementById('nombreRubro').value = rubro.nombre || '';
        document.getElementById('codigoProyecto').value = rubro.codigoProyecto || '';
        document.getElementById('nombreProyecto').value = rubro.nombreProyecto || '';
        
        // Seleccionar automáticamente el proyecto correspondiente
        if (rubro.idProyecto) {
            const proyectoSelect = document.getElementById('proyectoSelect');
            proyectoSelect.value = rubro.idProyecto;
        }
    } else {
        // Limpiar campos si no se encuentra el rubro
        document.getElementById('codigoRubro').value = '';
        document.getElementById('nombreRubro').value = '';
        document.getElementById('codigoProyecto').value = '';
        document.getElementById('nombreProyecto').value = '';
    }
};

// Función para autocompletar campos cuando se selecciona un proyecto
const autocompletarCamposProyecto = (proyectoId) => {
    const proyecto = proyectosDisponibles.find(p => p.id == proyectoId);
    
    if (proyecto) {
        document.getElementById('codigoProyecto').value = proyecto.codigo || '';
        document.getElementById('nombreProyecto').value = proyecto.nombre || '';
    } else {
        // Limpiar campos si no se encuentra el proyecto
        document.getElementById('codigoProyecto').value = '';
        document.getElementById('nombreProyecto').value = '';
    }
};

// Función global para editar donación
window.editarDonacion = async (donacion) => {
    try {
        const response = await getData(DONACIONES_GET_BY_ID_API(donacion.id));
        
        if (response && response.data) {
            const donacionCompleta = response.data;
            
            document.getElementById('donacionModalLabel').textContent = 'Editar Donación';
            document.getElementById('donacionId').value = donacionCompleta.id || '';
            document.getElementById('montoDonacion').value = donacionCompleta.monto || '';
            
            // Formatear fecha para datetime-local
            if (donacionCompleta.fechaDonacion) {
                const fecha = new Date(donacionCompleta.fechaDonacion);
                const fechaFormateada = fecha.toISOString().slice(0, 16);
                document.getElementById('fechaDonacion').value = fechaFormateada;
            }
            
            document.getElementById('nombreDonante').value = donacionCompleta.nombreDonante || '';
            document.getElementById('activeDonacion').checked = donacionCompleta.active === 1 || donacionCompleta.active === true;
            
            // Cargar rubros y proyectos, luego seleccionar los correspondientes
            await cargarRubrosEnSelect();
            await cargarProyectosEnSelect();
            
            document.getElementById('rubroSelect').value = donacionCompleta.idRubro || '';
            autocompletarCamposRubro(donacionCompleta.idRubro);
            
            const modal = new bootstrap.Modal(document.getElementById('modalDonacion'));
            modal.show();
        } else {
            showError('No se pudieron obtener los datos de la donación');
        }
    } catch (error) {
        showError('Error al cargar datos de la donación: ' + error.message);
    }
};

// Función global para eliminar donación
window.eliminarDonacion = (donacion) => {
    showConfirmation(
        `¿Estás seguro de eliminar la donación de "${donacion.nombreDonante}" por Q ${donacion.monto || '0.00'}?`,
        "Sí, eliminar",
        "Cancelar"
    ).then(async (confirmado) => {
        if (confirmado) {
            try {
                const response = await deleteData(`${DONACIONES_API}/eliminar/${donacion.id}`);
                
                if (response && response.success) {
                    showSuccess('Donación eliminada correctamente');
                    await obtenerTodasLasDonaciones();
                } else {
                    showError(response?.message || 'Error al eliminar la donación');
                }
            } catch (error) {
                showError('Error al eliminar la donación: ' + error.message);
            }
        }
    });
};

// Función para guardar donación
window.guardarDonacion = async () => {
    const form = document.getElementById('formDonacion');
    
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }
    
    try {
        const donacionId = document.getElementById('donacionId').value;
        const idRubro = document.getElementById('rubroSelect').value;
        const isEdit = donacionId > 0;
        
        if (!idRubro) {
            showError('Debe seleccionar un rubro');
            return;
        }
        
        const rubroSeleccionado = rubrosDisponibles.find(r => r.id == idRubro);
        
        const donacionData = {
            id: donacionId || 0,
            idRubro: parseInt(idRubro),
            monto: parseFloat(document.getElementById('montoDonacion').value),
            fechaDonacion: document.getElementById('fechaDonacion').value,
            nombreDonante: document.getElementById('nombreDonante').value,
            active: document.getElementById('activeDonacion').checked ? 1 : 0,
            codigoRubro: rubroSeleccionado?.codigo || '',
            nombreRubro: rubroSeleccionado?.nombre || '',
            codigoProyecto: rubroSeleccionado?.codigoProyecto || '',
            nombreProyecto: rubroSeleccionado?.nombreProyecto || ''
        };
        
        let response;
        if (isEdit) {
            response = await putData(`${DONACIONES_API}/actualizar/${donacionId}`, donacionData);
        } else {
            response = await postData(`${DONACIONES_API}/crear`, donacionData);
        }
        
        if (response && response.success) {
            showSuccess(response.message || (isEdit ? 'Donación actualizada' : 'Donación creada'));
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalDonacion'));
            modal.hide();
            await obtenerTodasLasDonaciones();
        } else {
            showError(response?.message || 'Error al guardar la donación');
        }
    } catch (error) {
        showError('Error al guardar la donación: ' + error.message);
    }
};

// Configurar eventos
document.addEventListener('DOMContentLoaded', function() {
    // Evento para el botón de guardar
    document.getElementById('btnGuardarDonacion').addEventListener('click', window.guardarDonacion);
    
    // Evento cuando se selecciona un rubro
    document.getElementById('rubroSelect').addEventListener('change', function() {
        autocompletarCamposRubro(this.value);
    });
    
    // Evento cuando se selecciona un proyecto
    document.getElementById('proyectoSelect').addEventListener('change', function() {
        autocompletarCamposProyecto(this.value);
    });
    
    // Evento cuando se abre el modal
    document.getElementById('modalDonacion').addEventListener('show.bs.modal', async function () {
        if (!document.getElementById('donacionId').value) {
            document.getElementById('donacionModalLabel').textContent = 'Nueva Donación';
            
            try {
                // Cargar rubros y proyectos
                await cargarRubrosEnSelect();
                await cargarProyectosEnSelect();
                
                // Establecer fecha actual por defecto
                document.getElementById('fechaDonacion').value = new Date().toISOString().slice(0, 16);
                
                // Limpiar campos
                document.getElementById('rubroSelect').value = '';
                document.getElementById('proyectoSelect').value = '';
                autocompletarCamposRubro('');
                autocompletarCamposProyecto('');
            } catch (error) {
                console.error('Error al inicializar modal:', error);
            }
        }
    });
    
    // Evento cuando se cierra el modal
    document.getElementById('modalDonacion').addEventListener('hidden.bs.modal', function () {
        document.getElementById('formDonacion').reset();
        document.getElementById('donacionId').value = '';
        document.getElementById('rubroSelect').value = '';
        document.getElementById('proyectoSelect').value = '';
        autocompletarCamposRubro('');
        autocompletarCamposProyecto('');
    });
});

// Inicializar
window.onload = async () => {
    await obtenerTodasLasDonaciones();
};