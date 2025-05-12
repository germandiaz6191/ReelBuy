window.QuillFunctions = {
    createQuill: function (quillElement, toolBar, readOnly, placeholder, theme, debugLevel) {
        try {
            var options = {
                debug: debugLevel,
                modules: {
                    toolbar: toolBar ? toolBar : false
                },
                placeholder: placeholder,
                readOnly: readOnly,
                theme: theme
            };

            var targetElement = quillElement;
            
            // Si es un string, usar querySelector
            if (typeof quillElement === 'string') {
                targetElement = document.querySelector("#" + quillElement);
            }
            
            // Crear la instancia de Quill
            var quill = new Quill(targetElement, options);
            window.quillEditors = window.quillEditors || {};
            window.quillEditors[quillElement] = quill;
            return true;
        } catch (error) {
            console.error("Error creating Quill instance:", error);
            return false;
        }
    },
    
    getQuillContent: function (quillElement) {
        try {
            var targetElement = this._getElement(quillElement);
            if (!targetElement) {
                return "";
            }
            
            var editorElement = targetElement.querySelector(".ql-editor");
            return editorElement ? editorElement.innerHTML : "";
        } catch (error) {
            console.error("Error getting Quill content:", error);
            return "";
        }
    },
    
    getQuillHTML: function (quillElement) {
        try {
            var targetElement = this._getElement(quillElement);
            if (!targetElement) {
                return "";
            }
            
            var editorElement = targetElement.querySelector(".ql-editor");
            return editorElement ? editorElement.innerHTML : "";
        } catch (error) {
            console.error("Error getting Quill HTML:", error);
            return "";
        }
    },
    
    getQuillText: function (quillElement) {
        try {
            var targetElement = this._getElement(quillElement);
            if (!targetElement) {
                return "";
            }
            
            var editorElement = targetElement.querySelector(".ql-editor");
            return editorElement ? editorElement.textContent : "";
        } catch (error) {
            console.error("Error getting Quill text:", error);
            return "";
        }
    },
    
    loadQuillHTMLContent: function (quillElement, quillHTMLContent) {
        try {
            console.log("[DEBUG] loadQuillHTMLContent llamado");
            console.log("[DEBUG] Elemento:", quillElement);
            console.log("[DEBUG] Contenido HTML a cargar:", quillHTMLContent);
            var targetElement = this._getElement(quillElement);
            if (!targetElement) {
                console.error("Target element not found:", quillElement);
                return false;
            }

            var quill = Quill.find(targetElement);
            if (quill) {
                console.log("[DEBUG] Quill instance encontrada, cargando contenido...");
                var delta = quill.clipboard.convert(quillHTMLContent || "");
                quill.setContents(delta);
                return true;
            } else {
                console.error("Quill instance not found for target element");
                return false;
            }
        } catch (error) {
            console.error("Error loading Quill HTML content:", error);
            return false;
        }
    },
    
    enableQuillEditor: function (quillElement, mode) {
        try {
            var targetElement = this._getElement(quillElement);
            if (!targetElement) {
                return false;
            }
            
            var quill = Quill.find(targetElement);
            if (quill) {
                quill.enable(mode);
                return true;
            }
            return false;
        } catch (error) {
            console.error("Error enabling Quill editor:", error);
            return false;
        }
    },
    
    insertQuillImage: function (quillElement, imageURL) {
        try {
            var element = this._getElement(quillElement);
            if (!element) return false;
            
            var quill = Quill.find(element);
            if (quill) {
                var range = quill.getSelection();
                var imagePosition = range ? range.index : 0;
                quill.insertEmbed(imagePosition, 'image', imageURL, 'user');
                return true;
            }
            return false;
        } catch (error) {
            console.error("Error inserting Quill image:", error);
            return false;
        }
    },
    
    // Función auxiliar para obtener el elemento DOM
    _getElement: function (quillElement) {
        if (!quillElement) {
            return null;
        }
        
        if (typeof quillElement === 'string') {
            return document.getElementById(quillElement);
        }
        
        return quillElement;
    }
};

window.setupQuillChangeDetection = function (quillEditorId, dotNetHelper) {
    try {
                
        // Variables para el seguimiento de intentos
        var maxAttempts = 10;
        var attemptCount = 0;
        var observerSetup = false;
        
        // Función para inicializar el observador
        function initializeObserver() {
            attemptCount++;
            
            if (observerSetup) {
                console.log("Observador ya configurado para:", quillEditorId);
                return;
            }
            
            if (attemptCount > maxAttempts) {
                console.error("Número máximo de intentos alcanzado para configurar el observador:", quillEditorId);
                return;
            }
            
            try {
                const editorContainer = document.getElementById(quillEditorId);
                if (!editorContainer) {
                    console.warn("Editor no encontrado con ID:", quillEditorId, "- Intento", attemptCount, "de", maxAttempts);
                    // Intentar nuevamente después de un retraso
                    setTimeout(initializeObserver, 300);
                    return;
                }

                // Buscar el elemento que contiene el editor Quill dentro del contenedor
                const quillContainer = editorContainer.querySelector('.ql-container');
                if (!quillContainer) {
                    console.warn("Contenedor Quill no encontrado dentro del editor - Intento", attemptCount, "de", maxAttempts);
                    // Intentar nuevamente después de un retraso
                    setTimeout(initializeObserver, 300);
                    return;
                }

                // Configurar un observador de mutación para detectar cambios en el contenido
                const observer = new MutationObserver(function(mutations) {
                    try {
                        console.log("Cambio detectado en el editor", quillEditorId);
                        // Notificar a .NET sobre el cambio
                        if (dotNetHelper) {
                            dotNetHelper.invokeMethodAsync('HandleContentChange')
                                .catch(function(error) {
                                    console.error("Error al notificar cambios:", error);
                                });
                        } else {
                            console.warn("dotNetHelper es null o indefinido");
                        }
                    } catch (error) {
                        console.error("Error en el callback del observador:", error);
                    }
                });

                // Configurar el observador para observar cambios en el contenido del editor
                try {
                    observer.observe(quillContainer, {
                        childList: true,
                        subtree: true,
                        characterData: true
                    });
                    
                    observerSetup = true;
                    
                    // Almacenar el observador para poder desconectarlo si es necesario
                    if (!window.quillObservers) {
                        window.quillObservers = {};
                    }
                    
                    // Si ya existía un observador previo, desconectarlo
                    if (window.quillObservers[quillEditorId]) {
                        try {
                            window.quillObservers[quillEditorId].disconnect();
                        } catch (error) {
                            console.warn("Error al desconectar observador anterior:", error);
                        }
                    }
                    
                    window.quillObservers[quillEditorId] = observer;
                    
                } catch (observeError) {
                    console.error("Error al configurar el observador:", observeError);
                }
            } catch (error) {
                console.error("Error en initializeObserver:", error);
                // Intentar nuevamente después de un retraso
                setTimeout(initializeObserver, 300);
            }
        }
        
        // Iniciar proceso de configuración con un pequeño retraso inicial
        setTimeout(initializeObserver, 100);
        
    } catch (error) {
        console.error("Error al configurar detección de cambios:", error);
    }
}; 
