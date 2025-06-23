
        .registration - container {
background: linear - gradient(135deg, #667eea 0%, #764ba2 100%);
            min - height: 100vh;
display: flex;
    align - items: center;
    justify - content: center;
padding: 20px;
}

        .registration - card {
background: #ffffff;
            border - radius: 20px;
    box - shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
padding: 0;
overflow: hidden;
    max - width: 900px;
width: 100 %;
}

        .registration - header {
background: linear - gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: white;
padding: 40px;
    text - align: center;
position: relative;
}

        .registration - header::before {
content: '';
position: absolute;
top: 0;
left: 0;
right: 0;
bottom: 0;
background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 20"><defs><radialGradient id="a" cx="50%" cy="0%" r="100%"><stop offset="0%" stop-color="white" stop-opacity="0.1"/><stop offset="100%" stop-color="white" stop-opacity="0"/></radialGradient></defs><rect fill="url(%23a)" width="100" height="20"/></svg>');
opacity: 0.5;
}

        .registration - header h2 {
            margin: 0;
font - size: 2.2rem;
font - weight: 700;
position: relative;
z - index: 1;
        }

        .registration - header p {
            margin: 10px 0 0 0;
opacity: 0.9;
font - size: 1.1rem;
position: relative;
z - index: 1;
        }

        .registration - icon {
background: rgba(255, 255, 255, 0.2);
width: 80px;
height: 80px;
    border - radius: 50 %;
display: flex;
    align - items: center;
    justify - content: center;
margin: 0 auto 20px;
position: relative;
    z - index: 1;
}

        .registration - body {
padding: 40px;
}

        .form - floating {
    margin - bottom: 20px;
}

        .form - floating > .form - control {
border: 2px solid #e5e7eb;
            border - radius: 12px;
padding: 1rem 0.75rem;
    font - size: 1rem;
transition: all 0.3s ease;
background: #f8fafc;
        }

        .form - floating > .form - control:focus {
            border-color: #2563eb;
            background: #ffffff;
            box - shadow: 0 0 0 0.2rem rgba(37, 99, 235, 0.15);
        }

        .form - floating > label {
color: #6b7280;
            font - weight: 500;
padding: 1rem 0.75rem;
}

        .btn - register {
background: linear - gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            border: none;
padding: 15px 40px;
    font - size: 1.1rem;
    font - weight: 600;
    border - radius: 12px;
color: white;
transition: all 0.3s ease;
    box - shadow: 0 4px 15px rgba(37, 99, 235, 0.3);
}

        .btn - register:hover {
            transform: translateY(-2px);
box - shadow: 0 6px 20px rgba(37, 99, 235, 0.4);
background: linear - gradient(135deg, #1d4ed8 0%, #1e40af 100%);
            color: white;
        }

        .btn - register:active {
            transform: translateY(0);
        }

        .success - message {
background: linear - gradient(135deg, #10b981 0%, #059669 100%);
            color: white;
padding: 20px;
    border - radius: 12px;
    text - align: center;
    margin - bottom: 20px;
border: none;
}

        .success - icon {
    font - size: 2rem;
    margin - bottom: 10px;
}

        .error - message {
background: linear - gradient(135deg, #ef4444 0%, #dc2626 100%);
            color: white;
padding: 15px;
    border - radius: 12px;
    margin - bottom: 20px;
border: none;
}

        .input - group - icon {
position: relative;
}

        .input - group - icon i {
            position: absolute;
left: 15px;
top: 50 %;
transform: translateY(-50 %);
color: #6b7280;
            z - index: 3;
        }

        .input - group - icon.form - control {
    padding - left: 45px;
}

        .password - toggle {
position: absolute;
right: 15px;
top: 50 %;
transform: translateY(-50 %);
background: none;
border: none;
color: #6b7280;
            cursor: pointer;
    z - index: 3;
}

        .password - toggle:hover {
            color: #2563eb;
        }

        .form - row {
display: grid;
    grid - template - columns: 1fr 1fr;
gap: 20px;
}

@media(max - width: 768px) {
            .form - row {
        grid - template - columns: 1fr;
    }
            
            .registration - header {
    padding: 30px 20px;
    }
            
            .registration - body {
    padding: 30px 20px;
    }
            
            .registration - header h2 {
        font - size: 1.8rem;
    }
}

        .validation - error {
color: #ef4444;
            font - size: 0.875rem;
    margin - top: 5px;
display: flex;
    align - items: center;
gap: 5px;
}

        .loading - overlay {
position: absolute;
top: 0;
left: 0;
right: 0;
bottom: 0;
background: rgba(255, 255, 255, 0.9);
display: flex;
    align - items: center;
    justify - content: center;
    z - index: 1000;
    border - radius: 20px;
}

        .spinner - custom {
width: 40px;
height: 40px;
border: 4px solid #e5e7eb;
            border - top: 4px solid #2563eb;
            border - radius: 50 %;
animation: spin 1s linear infinite;
}

@keyframes spin
{
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
}
  