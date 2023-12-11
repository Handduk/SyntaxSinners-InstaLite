import { MemoryRouter } from "react-router-dom";
import { Navbar } from "../components/Navbar";
import { expect, it } from "vitest";
import { render, screen } from "@testing-library/react"


it("renders Navbar with buttons and checking if components exists", () => {
  render(
    <MemoryRouter>
      <Navbar />
    </MemoryRouter>
  );

  const homeButton = screen.getByText(/InstaLite/i);
  const uploadButton = screen.getByText(/Upload/i);

  expect(homeButton).toContain(/InstaLite/i);
  expect(uploadButton).toContain(/Upload/i);
});