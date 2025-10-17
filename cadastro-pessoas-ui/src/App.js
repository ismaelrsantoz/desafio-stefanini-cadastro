import React, { useState } from 'react';
import './App.css';
import Login from './components/Login';
import Dashboard from './components/Dashboard';
import PessoaForm from './components/PessoaForm';
import PessoasList from './components/PessoasList';
import PessoaFilter from './components/PessoaFilter';
import { Container, Paper, Button, Box } from '@mui/material';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import LogoutIcon from '@mui/icons-material/Logout';

// Centralizei a URL da API aqui para facilitar a manutenção e o deploy.
const apiUrl = process.env.REACT_APP_API_URL || "http://localhost:5182";

function App() {
  const [token, setToken] = useState(null);
  const [view, setView] = useState('dashboard');
  const [pessoas, setPessoas] = useState([]);
  const [pessoaParaEditar, setPessoaParaEditar] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const getAuthHeaders = () => ({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  });

  const fetchPessoas = (filters = {}) => {
    if (!token) return;
    setIsLoading(true);
    setHasSearched(true);
    const params = new URLSearchParams();
    if (filters.nome) params.append('nome', filters.nome);
    if (filters.cpf) params.append('cpf', filters.cpf);
    
    // CORREÇÃO: Utiliza a variável apiUrl
    fetch(`${apiUrl}/api/v1/Pessoas?${params.toString()}`, { headers: getAuthHeaders() })
      .then(res => res.json())
      .then(data => { setPessoas(data); setIsLoading(false); })
      .catch(() => { setIsLoading(false); toast.error("Falha ao buscar dados da API."); });
  };
  
  const handleSave = (pessoa) => {
    const isEditing = !!pessoa.id;
    // CORREÇÃO: Utiliza a variável apiUrl
    const url = isEditing ? `${apiUrl}/api/v1/Pessoas/${pessoa.id}` : `${apiUrl}/api/v1/Pessoas`;
    const method = isEditing ? 'PUT' : 'POST';

    fetch(url, { method, headers: getAuthHeaders(), body: JSON.stringify(pessoa) })
      .then(async response => {
        if (response.ok) {
          toast.success(`Pessoa ${isEditing ? 'atualizada' : 'cadastrada'}!`);
          setPessoaParaEditar(null);
          setView('dashboard');
        } else {
          const errorMessage = await response.text();
          toast.error(errorMessage || 'Erro ao salvar. Verifique os dados.');
        }
      });
  };
  
  const handleDelete = (id) => {
    if (window.confirm('Tem certeza?')) {
      // CORREÇÃO: Utiliza a variável apiUrl
      fetch(`${apiUrl}/api/v1/Pessoas/${id}`, { method: 'DELETE', headers: getAuthHeaders() })
        .then(res => {
          if (res.ok) {
            toast.success('Pessoa excluída!');
            setPessoas(pessoas.filter(p => p.id !== id));
          } else {
            toast.error('Erro ao excluir.');
          }
        });
    }
  };

  const handleLogout = () => {
    setToken(null);
    setView('dashboard');
    setPessoas([]);
    setHasSearched(false);
  };

  const handleNavigate = (targetView) => {
    setView(targetView);
    setPessoas([]);
    setHasSearched(false);
  };

  if (!token) {
    return (
        <>
            <ToastContainer position="top-right" autoClose={3000} />
            <Login onLoginSuccess={setToken} />
        </>
    );
  }

  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} />
      <Box sx={{ position: 'absolute', top: 16, right: 16, zIndex: 1200 }}>
        <Button 
            variant="contained" 
            color="error" 
            onClick={handleLogout}
            startIcon={<LogoutIcon />}
        >
          Sair
        </Button>
      </Box>
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        
        {view !== 'dashboard' && (
          <Button startIcon={<ArrowBackIcon />} onClick={() => handleNavigate('dashboard')} sx={{ mb: 2 }}>
            Voltar ao Menu
          </Button>
        )}

        {view === 'dashboard' && <Dashboard onNavigate={handleNavigate} />}
        
        {view === 'cadastro' && (
          <Paper elevation={3} sx={{ p: 3, mb: 4 }}>
            <PessoaForm onSave={handleSave} pessoaParaEditar={pessoaParaEditar} onCancelEdit={() => handleNavigate('dashboard')} />
          </Paper>
        )}
        
        {view === 'consulta' && (
          <>
            <PessoaFilter onSearch={fetchPessoas} />
            {hasSearched && (
              <PessoasList 
                pessoas={pessoas} 
                onDelete={handleDelete} 
                onEdit={(pessoa) => { setPessoaParaEditar(pessoa); setView('cadastro'); }} 
                isLoading={isLoading} 
              />
            )}
          </>
        )}
      </Container>
    </>
  );
}

export default App;
