## Developer Guidelines for ALSI.PrimitiveValidation

This document provides guidelines for contributing to the ALSI.PrimitiveValidation project, detailing performance requirements, code style, and contribution practices.

### Code Style and Formatting

ALSI.PrimitiveValidation adheres to strict coding standards to maintain code quality and consistency across the project. We use the following tools:

- **StyleCop**: Ensures compliance with coding style rules, including naming conventions, documentation requirements, and code layout.
- **CSharpier**: An opinionated code formatter that enforces a consistent code style across the project. All code should be formatted with CSharpier before submission.

### Code Structure

- **Namespace**: All public and internal classes and methods are housed under the `ALSI.PrimitiveValidation` namespace.
- **Core Design**:
  - The primary goal of the package is to provide validation attributes for primitive types.
  - Secondary goals may include backporting net8.0 features to ensure speed is maintained.

### Contribution Guidelines

Contributions to ALSI.PrimitiveValidation are welcome! Please follow these guidelines to ensure smooth collaboration:

#### 1. **Build and Test the Project**

- Ensure that the project builds without errors.
- Run the unit tests to verify that everything works as expected:

     ```bash
     dotnet build
     dotnet test
     ```

#### 2. **Follow Coding Standards**

- Ensure your code follows the StyleCop rules. Violations will be flagged during code review and the build process.
- Format your code with CSharpier before committing:

     ```bash
     dotnet csharpier .
     ```

#### 3. **Write Unit Tests**

- Any new features or bug fixes should be accompanied by unit tests that verify their correctness.
- Place tests in the appropriate `UnitTests` project within the `ALSI.PrimitiveValidation.UnitTests` namespace.

#### 4. **Submit a Pull Request**

- Push your changes to your fork.
- Submit a pull request to the `dev` branch of the main repository.
- Provide a clear description of the changes, including the problem being solved or feature being added.
